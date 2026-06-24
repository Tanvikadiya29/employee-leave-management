using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.API.DTOs.Auth;

public class RegisterDto
{
    [Required] public string FirstName { get; set; } = string.Empty;
    [Required] public string LastName  { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$",
        ErrorMessage = "Password must contain uppercase, lowercase, digit, and special character.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Range(1, 2, ErrorMessage = "RoleId must be 1 (Admin) or 2 (Employee).")]
    public int RoleId { get; set; }

    public string?   Department    { get; set; }
    public string?   Designation   { get; set; }
    public DateOnly? DateOfJoining { get; set; }
}
