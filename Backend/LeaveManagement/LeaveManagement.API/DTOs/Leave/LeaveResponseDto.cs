namespace LeaveManagement.API.DTOs.Leave;

public class LeaveResponseDto
{
    public int       Id             { get; set; }
    public int       EmployeeId     { get; set; }
    public string    EmployeeName   { get; set; } = string.Empty;
    public string    EmployeeEmail  { get; set; } = string.Empty;
    public string?   Department     { get; set; }
    public DateOnly  FromDate       { get; set; }
    public DateOnly  ToDate         { get; set; }
    public int       TotalDays      => ToDate.DayNumber - FromDate.DayNumber + 1;
    public string    Reason         { get; set; } = string.Empty;
    public string    Status         { get; set; } = string.Empty;
    public string?   Remarks        { get; set; }
    public string?   ReviewedByName { get; set; }
    public DateTime? ReviewedAt     { get; set; }
    public DateTime  CreatedAt      { get; set; }
}
