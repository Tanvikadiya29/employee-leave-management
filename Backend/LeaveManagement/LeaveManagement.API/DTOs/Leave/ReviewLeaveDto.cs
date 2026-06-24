using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.API.DTOs.Leave;

public class ReviewLeaveDto
{
    [MaxLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
    public string? Remarks { get; set; }
}
