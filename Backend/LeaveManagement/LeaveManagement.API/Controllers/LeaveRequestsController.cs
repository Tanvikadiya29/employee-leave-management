using LeaveManagement.API.DTOs.Common;
using LeaveManagement.API.DTOs.Leave;
using LeaveManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestService _leaveService;

    public LeaveRequestsController(ILeaveRequestService leaveService)
    {
        _leaveService = leaveService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginationParams pagination,
        [FromQuery] string? status = null)
    {
        var result = await _leaveService.GetAllAsync(pagination, status);
        return Ok(result);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetMyLeaves([FromQuery] PaginationParams pagination)
    {
        int userId = GetCurrentUserId();
        var result = await _leaveService.GetMyLeavesAsync(userId, pagination);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        int userId = GetCurrentUserId();
        string role = GetCurrentUserRole();

        var result = await _leaveService.GetByIdAsync(id, userId, role);
        if (!result.Success)
            return result.Message.Contains("Access") ? Forbid() : NotFound(new { result.Message });

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Create([FromBody] CreateLeaveRequestDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        int userId = GetCurrentUserId();
        var result = await _leaveService.CreateAsync(dto, userId);
        if (!result.Success)
            return BadRequest(new { result.Message });

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id:int}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(int id, [FromBody] ReviewLeaveDto dto)
    {
        int adminId = GetCurrentUserId();
        var result = await _leaveService.ApproveAsync(id, adminId, dto);
        if (!result.Success)
            return BadRequest(new { result.Message });

        return Ok(result);
    }

    [HttpPut("{id:int}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reject(int id, [FromBody] ReviewLeaveDto dto)
    {
        int adminId = GetCurrentUserId();
        var result = await _leaveService.RejectAsync(id, adminId, dto);
        if (!result.Success)
            return BadRequest(new { result.Message });

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Cancel(int id)
    {
        int userId = GetCurrentUserId();
        var result = await _leaveService.CancelAsync(id, userId);
        if (!result.Success)
            return BadRequest(new { result.Message });

        return Ok(result);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst("userId")?.Value;
        return int.TryParse(claim, out int id) ? id : 0;
    }

    private string GetCurrentUserRole()
        => User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
}
