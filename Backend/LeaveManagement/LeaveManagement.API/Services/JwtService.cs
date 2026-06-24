using LeaveManagement.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LeaveManagement.API.Services;

public class JwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public (string Token, DateTime ExpiresAt) GenerateToken(User user)
    {
        var settings   = _config.GetSection("JwtSettings");
        var secretKey  = settings["SecretKey"]!;
        var issuer     = settings["Issuer"]!;
        var audience   = settings["Audience"]!;
        var expiryMins = int.Parse(settings["ExpiryInMinutes"]!);
        var expiresAt  = DateTime.UtcNow.AddMinutes(expiryMins);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new(ClaimTypes.Role,               user.Role.RoleName),
            new("userId",                      user.Id.ToString()),
            new("fullName",                    $"{user.FirstName} {user.LastName}")
        };

        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            notBefore:          DateTime.UtcNow,
            expires:            expiresAt,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
