using LeaveManagement.API.DTOs.Common;
using LeaveManagement.API.DTOs.Employee;

namespace LeaveManagement.API.Services.Interfaces;

public interface IEmployeeService
{
    Task<ApiResponse<List<EmployeeResponseDto>>> GetAllAsync(PaginationParams pagination);
    Task<ApiResponse<EmployeeResponseDto>>       GetByIdAsync(int id);
    Task<ApiResponse<EmployeeResponseDto>>       CreateAsync(CreateEmployeeDto dto);
    Task<ApiResponse<EmployeeResponseDto>>       UpdateAsync(int id, UpdateEmployeeDto dto);
    Task<ApiResponse<bool>>                      DeleteAsync(int id);
}
