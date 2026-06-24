namespace LeaveManagement.API.Models;

public enum LeaveStatus
{
    Pending,
    Approved,
    Rejected
}

public class LeaveRequest
{
    public int         Id         { get; set; }
    public int         EmployeeId { get; set; }
    public DateOnly    FromDate   { get; set; }
    public DateOnly    ToDate     { get; set; }
    public string      Reason     { get; set; } = string.Empty;
    public LeaveStatus Status     { get; set; } = LeaveStatus.Pending;
    public int?        ReviewedBy { get; set; }
    public DateTime?   ReviewedAt { get; set; }
    public string?     Remarks    { get; set; }
    public DateTime    CreatedAt  { get; set; } = DateTime.UtcNow;
    public DateTime    UpdatedAt  { get; set; } = DateTime.UtcNow;

    public User  Employee { get; set; } = null!;
    public User? Reviewer { get; set; }
}
