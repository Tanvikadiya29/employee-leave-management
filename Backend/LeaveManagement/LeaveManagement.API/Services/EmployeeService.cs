using LeaveManagement.API.Data;
using LeaveManagement.API.DTOs.Common;
using LeaveManagement.API.DTOs.Employee;
using LeaveManagement.API.Models;
using LeaveManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.API.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;

    public EmployeeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<EmployeeResponseDto>>> GetAllAsync(PaginationParams p)
    {
        var query = _context.Users
            .Include(u => u.Role)
            .Where(u => u.Role.RoleName == "Employee" && u.IsActive)
            .OrderBy(u => u.FirstName);

        int total = await query.CountAsync();
        var users = await query
            .Skip((p.Page - 1) * p.PageSize)
            .Take(p.PageSize)
            .Select(u => MapToDto(u))
            .ToListAsync();

        return ApiResponse<List<EmployeeResponseDto>>.Paginated(users, total, p.Page, p.PageSize);
    }

    public async Task<ApiResponse<EmployeeResponseDto>> GetByIdAsync(int id)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

        if (user == null)
            return ApiResponse<EmployeeResponseDto>.Fail("Employee not found.");

        return ApiResponse<EmployeeResponseDto>.Ok(MapToDto(user));
    }

    public async Task<ApiResponse<EmployeeResponseDto>> CreateAsync(CreateEmployeeDto dto)
    {
        bool emailExists = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());

        if (emailExists)
            return ApiResponse<EmployeeResponseDto>.Fail("An employee with this email already exists.");

        var employeeRoleId = await _context.Roles
            .Where(r => r.RoleName == "Employee")
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

        var user = new User
        {
            FirstName     = dto.FirstName.Trim(),
            LastName      = dto.LastName.Trim(),
            Email         = dto.Email.Trim().ToLower(),
            PasswordHash  = BCrypt.Net.BCrypt.HashPassword(dto.Password, 10),
            RoleId        = employeeRoleId,
            Department    = dto.Department?.Trim(),
            Designation   = dto.Designation?.Trim(),
            DateOfJoining = dto.DateOfJoining,
            IsActive      = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        await _context.Entry(user).Reference(u => u.Role).LoadAsync();

        return ApiResponse<EmployeeResponseDto>.Ok(MapToDto(user), "Employee created successfully.");
    }

    public async Task<ApiResponse<EmployeeResponseDto>> UpdateAsync(int id, UpdateEmployeeDto dto)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return ApiResponse<EmployeeResponseDto>.Fail("Employee not found.");

        if (dto.FirstName     != null) user.FirstName     = dto.FirstName.Trim();
        if (dto.LastName      != null) user.LastName      = dto.LastName.Trim();
        if (dto.Department    != null) user.Department    = dto.Department.Trim();
        if (dto.Designation   != null) user.Designation   = dto.Designation.Trim();
        if (dto.DateOfJoining != null) user.DateOfJoining = dto.DateOfJoining;
        if (dto.IsActive      != null) user.IsActive      = dto.IsActive.Value;

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ApiResponse<EmployeeResponseDto>.Ok(MapToDto(user), "Employee updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return ApiResponse<bool>.Fail("Employee not found.");

        user.IsActive  = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Employee deactivated successfully.");
    }

    private static EmployeeResponseDto MapToDto(User u) => new()
    {
        Id            = u.Id,
        FirstName     = u.FirstName,
        LastName      = u.LastName,
        Email         = u.Email,
        RoleName      = u.Role?.RoleName ?? string.Empty,
        Department    = u.Department,
        Designation   = u.Designation,
        DateOfJoining = u.DateOfJoining,
        IsActive      = u.IsActive,
        CreatedAt     = u.CreatedAt
    };
}
