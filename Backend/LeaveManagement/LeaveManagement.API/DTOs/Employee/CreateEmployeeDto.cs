using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.API.DTOs.Employee;

public class CreateEmployeeDto
{
    [Required] public string FirstName { get; set; } = string.Empty;
    [Required] public string LastName  { get; set; } = string.Empty;
    [Required][EmailAddress] public string Email    { get; set; } = string.Empty;
    [Required][MinLength(8)] public string Password { get; set; } = string.Empty;
    public string?   Department    { get; set; }
    public string?   Designation   { get; set; }
    public DateOnly? DateOfJoining { get; set; }
}
