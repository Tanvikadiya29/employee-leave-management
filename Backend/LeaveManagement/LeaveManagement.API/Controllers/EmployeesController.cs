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

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination)
    {
        var result = await _employeeService.GetAllAsync(pagination);
        return Ok(result);
    }

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

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto)
    {
        var result = await _employeeService.UpdateAsync(id, dto);
        if (!result.Success)
            return NotFound(new { result.Message });

        return Ok(result);
    }

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
