using HR.Core.Entities;
using HR.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var departments = await _departmentService.GetAllAsync();
        return Ok(departments);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] Department department)
    {
        var createdDepartment = await _departmentService.CreateAsync(department);
        return CreatedAtAction(nameof(GetAll), new { id = createdDepartment.Id }, createdDepartment);
    }
}