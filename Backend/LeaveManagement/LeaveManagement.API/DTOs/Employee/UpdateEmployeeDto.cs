namespace LeaveManagement.API.DTOs.Employee;

public class UpdateEmployeeDto
{
    public string?   FirstName     { get; set; }
    public string?   LastName      { get; set; }
    public string?   Department    { get; set; }
    public string?   Designation   { get; set; }
    public DateOnly? DateOfJoining { get; set; }
    public bool?     IsActive      { get; set; }
}
