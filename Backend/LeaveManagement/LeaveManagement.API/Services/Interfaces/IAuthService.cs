using LeaveManagement.API.DTOs.Auth;

namespace LeaveManagement.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?>                                  LoginAsync(LoginDto dto);
    Task<(bool Success, string Message, int UserId)> RegisterAsync(RegisterDto dto);
}
