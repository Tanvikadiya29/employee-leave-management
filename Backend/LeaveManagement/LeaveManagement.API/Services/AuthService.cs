using LeaveManagement.API.Data;
using LeaveManagement.API.DTOs.Auth;
using LeaveManagement.API.Models;
using LeaveManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtService   _jwtService;

    public AuthService(AppDbContext context, JwtService jwtService)
    {
        _context    = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower() && u.IsActive);

        if (user == null) return null;

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!isPasswordValid) return null;

        var (token, expiresAt) = _jwtService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token     = token,
            Role      = user.Role.RoleName,
            UserId    = user.Id,
            FullName  = $"{user.FirstName} {user.LastName}",
            Email     = user.Email,
            ExpiresAt = expiresAt
        };
    }

    public async Task<(bool Success, string Message, int UserId)> RegisterAsync(RegisterDto dto)
    {
        bool emailExists = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());

        if (emailExists)
            return (false, "An account with this email already exists.", 0);

        bool roleExists = await _context.Roles.AnyAsync(r => r.Id == dto.RoleId);
        if (!roleExists)
            return (false, "Invalid role specified.", 0);

        var user = new User
        {
            FirstName     = dto.FirstName.Trim(),
            LastName      = dto.LastName.Trim(),
            Email         = dto.Email.Trim().ToLower(),
            PasswordHash  = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 10),
            RoleId        = dto.RoleId,
            Department    = dto.Department?.Trim(),
            Designation   = dto.Designation?.Trim(),
            DateOfJoining = dto.DateOfJoining,
            IsActive      = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return (true, "User registered successfully.", user.Id);
    }
}
