namespace LeaveManagement.API.DTOs.Employee;

public class EmployeeResponseDto
{
    public int       Id            { get; set; }
    public string    FirstName     { get; set; } = string.Empty;
    public string    LastName      { get; set; } = string.Empty;
    public string    FullName      => $"{FirstName} {LastName}";
    public string    Email         { get; set; } = string.Empty;
    public string    RoleName      { get; set; } = string.Empty;
    public string?   Department    { get; set; }
    public string?   Designation   { get; set; }
    public DateOnly? DateOfJoining { get; set; }
    public bool      IsActive      { get; set; }
    public DateTime  CreatedAt     { get; set; }
}
