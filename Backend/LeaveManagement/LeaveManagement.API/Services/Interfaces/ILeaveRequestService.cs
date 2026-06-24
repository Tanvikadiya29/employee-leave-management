using LeaveManagement.API.DTOs.Common;
using LeaveManagement.API.DTOs.Leave;

namespace LeaveManagement.API.Services.Interfaces;

public interface ILeaveRequestService
{
    Task<ApiResponse<List<LeaveResponseDto>>> GetAllAsync(PaginationParams p, string? statusFilter);
    Task<ApiResponse<List<LeaveResponseDto>>> GetMyLeavesAsync(int employeeId, PaginationParams p);
    Task<ApiResponse<LeaveResponseDto>>       GetByIdAsync(int id, int requestingUserId, string requestingUserRole);
    Task<ApiResponse<LeaveResponseDto>>       CreateAsync(CreateLeaveRequestDto dto, int employeeId);
    Task<ApiResponse<LeaveResponseDto>>       ApproveAsync(int id, int adminId, ReviewLeaveDto dto);
    Task<ApiResponse<LeaveResponseDto>>       RejectAsync(int id, int adminId, ReviewLeaveDto dto);
    Task<ApiResponse<bool>>                   CancelAsync(int id, int employeeId);
}
