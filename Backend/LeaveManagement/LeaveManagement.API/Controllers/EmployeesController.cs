using LeaveManagement.API.DTOs.Common;
using LeaveManagement.API.DTOs.Employee;
using LeaveManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>Get all active employees. Admin only. Supports pagination.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination)
    {
        var result = await _employeeService.GetAllAsync(pagination);
        return Ok(result);
    }

    /// <summary>Get employee by ID. Admin can view any; Employee can view own profile only.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (GetCurrentUserRole() == "Employee" && GetCurrentUserId() != id)
            return Forbid();

        var result = await _employeeService.GetByIdAsync(id);
        if (!result.Success)
            return NotFound(new { result.Message });

        return Ok(result);
    }

    /// <summary>Add a new employee. Admin only.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _employeeService.CreateAsync(dto);
        if (!result.Success)
            return BadRequest(new { result.Message });

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    /// <summary>Update employee details. Admin only.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto)
    {
        var result = await _employeeService.UpdateAsync(id, dto);
        if (!result.Success)
            return NotFound(new { result.Message });

        return Ok(result);
    }

    /// <summary>Soft-delete (deactivate) an employee. Admin only.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _employeeService.DeleteAsync(id);
        if (!result.Success)
            return NotFound(new { result.Message });

        return Ok(result);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst("userId")?.Value;
        return int.TryParse(claim, out int id) ? id : 0;
    }

    private string GetCurrentUserRole()
        => User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;
}
