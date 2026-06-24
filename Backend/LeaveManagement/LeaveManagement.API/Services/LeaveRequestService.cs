using LeaveManagement.API.Data;
using LeaveManagement.API.DTOs.Common;
using LeaveManagement.API.DTOs.Leave;
using LeaveManagement.API.Models;
using LeaveManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.API.Services;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly AppDbContext _context;

    public LeaveRequestService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<LeaveResponseDto>>> GetAllAsync(PaginationParams p, string? statusFilter)
    {
        var query = _context.LeaveRequests
            .Include(l => l.Employee).ThenInclude(u => u.Role)
            .Include(l => l.Reviewer)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(statusFilter)
            && Enum.TryParse<LeaveStatus>(statusFilter, true, out var parsedStatus))
        {
            query = query.Where(l => l.Status == parsedStatus);
        }

        query = query.OrderByDescending(l => l.CreatedAt);

        int total = await query.CountAsync();
        var items = await query
            .Skip((p.Page - 1) * p.PageSize)
            .Take(p.PageSize)
            .Select(l => MapToDto(l))
            .ToListAsync();

        return ApiResponse<List<LeaveResponseDto>>.Paginated(items, total, p.Page, p.PageSize);
    }

    public async Task<ApiResponse<List<LeaveResponseDto>>> GetMyLeavesAsync(int employeeId, PaginationParams p)
    {
        var query = _context.LeaveRequests
            .Include(l => l.Employee).ThenInclude(u => u.Role)
            .Include(l => l.Reviewer)
            .Where(l => l.EmployeeId == employeeId)
            .OrderByDescending(l => l.CreatedAt);

        int total = await query.CountAsync();
        var items = await query
            .Skip((p.Page - 1) * p.PageSize)
            .Take(p.PageSize)
            .Select(l => MapToDto(l))
            .ToListAsync();

        return ApiResponse<List<LeaveResponseDto>>.Paginated(items, total, p.Page, p.PageSize);
    }

    public async Task<ApiResponse<LeaveResponseDto>> GetByIdAsync(int id, int requestingUserId, string role)
    {
        var leave = await _context.LeaveRequests
            .Include(l => l.Employee).ThenInclude(u => u.Role)
            .Include(l => l.Reviewer)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (leave == null)
            return ApiResponse<LeaveResponseDto>.Fail("Leave request not found.");

        if (role == "Employee" && leave.EmployeeId != requestingUserId)
            return ApiResponse<LeaveResponseDto>.Fail("Access denied.");

        return ApiResponse<LeaveResponseDto>.Ok(MapToDto(leave));
    }

    public async Task<ApiResponse<LeaveResponseDto>> CreateAsync(CreateLeaveRequestDto dto, int employeeId)
    {
        var employee = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == employeeId && u.IsActive);
        if (employee == null)
            return ApiResponse<LeaveResponseDto>.Fail("Employee not found or inactive.");

        if (dto.ToDate < dto.FromDate)
            return ApiResponse<LeaveResponseDto>.Fail("End date must be on or after start date.");

        if (dto.FromDate < DateOnly.FromDateTime(DateTime.Today))
            return ApiResponse<LeaveResponseDto>.Fail("Leave start date cannot be in the past.");

        bool hasOverlap = await HasOverlapAsync(employeeId, dto.FromDate, dto.ToDate);
        if (hasOverlap)
            return ApiResponse<LeaveResponseDto>.Fail(
                "You already have an approved or pending leave request for overlapping dates.");

        var leave = new LeaveRequest
        {
            EmployeeId = employeeId,
            FromDate   = dto.FromDate,
            ToDate     = dto.ToDate,
            Reason     = dto.Reason.Trim(),
            Status     = LeaveStatus.Pending
        };

        _context.LeaveRequests.Add(leave);
        await _context.SaveChangesAsync();

        await _context.Entry(leave).Reference(l => l.Employee).LoadAsync();
        await _context.Entry(leave.Employee).Reference(u => u.Role).LoadAsync();

        return ApiResponse<LeaveResponseDto>.Ok(MapToDto(leave), "Leave request submitted successfully.");
    }

    public async Task<ApiResponse<LeaveResponseDto>> ApproveAsync(int id, int adminId, ReviewLeaveDto dto)
    {
        var leave = await _context.LeaveRequests
            .Include(l => l.Employee).ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (leave == null)
            return ApiResponse<LeaveResponseDto>.Fail("Leave request not found.");

        if (leave.Status != LeaveStatus.Pending)
            return ApiResponse<LeaveResponseDto>.Fail($"Cannot approve a leave that is already {leave.Status}.");

        leave.Status     = LeaveStatus.Approved;
        leave.ReviewedBy = adminId;
        leave.ReviewedAt = DateTime.UtcNow;
        leave.Remarks    = dto.Remarks?.Trim();
        leave.UpdatedAt  = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await _context.Entry(leave).Reference(l => l.Reviewer).LoadAsync();

        return ApiResponse<LeaveResponseDto>.Ok(MapToDto(leave), "Leave request approved.");
    }

    public async Task<ApiResponse<LeaveResponseDto>> RejectAsync(int id, int adminId, ReviewLeaveDto dto)
    {
        var leave = await _context.LeaveRequests
            .Include(l => l.Employee).ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (leave == null)
            return ApiResponse<LeaveResponseDto>.Fail("Leave request not found.");

        if (leave.Status != LeaveStatus.Pending)
            return ApiResponse<LeaveResponseDto>.Fail($"Cannot reject a leave that is already {leave.Status}.");

        leave.Status     = LeaveStatus.Rejected;
        leave.ReviewedBy = adminId;
        leave.ReviewedAt = DateTime.UtcNow;
        leave.Remarks    = dto.Remarks?.Trim();
        leave.UpdatedAt  = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await _context.Entry(leave).Reference(l => l.Reviewer).LoadAsync();

        return ApiResponse<LeaveResponseDto>.Ok(MapToDto(leave), "Leave request rejected.");
    }

    public async Task<ApiResponse<bool>> CancelAsync(int id, int employeeId)
    {
        var leave = await _context.LeaveRequests.FirstOrDefaultAsync(l => l.Id == id);

        if (leave == null)
            return ApiResponse<bool>.Fail("Leave request not found.");

        if (leave.EmployeeId != employeeId)
            return ApiResponse<bool>.Fail("You can only cancel your own leave requests.");

        if (leave.Status != LeaveStatus.Pending)
            return ApiResponse<bool>.Fail("Only pending leave requests can be cancelled.");

        _context.LeaveRequests.Remove(leave);
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Leave request cancelled successfully.");
    }

    private async Task<bool> HasOverlapAsync(
        int employeeId, DateOnly fromDate, DateOnly toDate, int? excludeId = null)
    {
        return await _context.LeaveRequests
            .Where(l => l.EmployeeId == employeeId
                     && (l.Status == LeaveStatus.Pending || l.Status == LeaveStatus.Approved)
                     && (excludeId == null || l.Id != excludeId)
                     && l.FromDate <= toDate
                     && l.ToDate   >= fromDate)
            .AnyAsync();
    }

    private static LeaveResponseDto MapToDto(LeaveRequest l) => new()
    {
        Id             = l.Id,
        EmployeeId     = l.EmployeeId,
        EmployeeName   = $"{l.Employee?.FirstName} {l.Employee?.LastName}",
        EmployeeEmail  = l.Employee?.Email ?? string.Empty,
        Department     = l.Employee?.Department,
        FromDate       = l.FromDate,
        ToDate         = l.ToDate,
        Reason         = l.Reason,
        Status         = l.Status.ToString(),
        Remarks        = l.Remarks,
        ReviewedByName = l.Reviewer != null
                         ? $"{l.Reviewer.FirstName} {l.Reviewer.LastName}"
                         : null,
        ReviewedAt     = l.ReviewedAt,
        CreatedAt      = l.CreatedAt
    };
}
