using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.API.DTOs.Leave;

public class CreateLeaveRequestDto
{
    [Required(ErrorMessage = "From date is required.")]
    public DateOnly FromDate { get; set; }

    [Required(ErrorMessage = "To date is required.")]
    public DateOnly ToDate   { get; set; }

    [Required(ErrorMessage = "Reason is required.")]
    [MinLength(5, ErrorMessage = "Reason must be at least 5 characters.")]
    [MaxLength(1000, ErrorMessage = "Reason cannot exceed 1000 characters.")]
    public string Reason { get; set; } = string.Empty;
}
