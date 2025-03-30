using AutoMapper;
using HR.Core.DTOs.EmployeeDTOs;
using HR.Core.Entities;
using HR.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HR.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IMapper _mapper;

    public EmployeesController(IEmployeeService employeeService, IMapper mapper)
    {
        _employeeService = employeeService;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var employees = await _employeeService.GetAllAsync(page, pageSize);
        var employeeDtos = _mapper.Map<List<EmployeeResponseDto>>(employees);
        return Ok(employeeDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (User.IsInRole("Employee") && int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value) != id)
            return Forbid();
        var employeeDto = _mapper.Map<EmployeeResponseDto>(employee);
        return Ok(employeeDto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
    {
        var employee = _mapper.Map<Employee>(dto);
        var createdEmployee = await _employeeService.CreateAsync(employee);
        var employeeDto = _mapper.Map<EmployeeResponseDto>(createdEmployee);
        return CreatedAtAction(nameof(GetById), new { id = employeeDto.Id }, employeeDto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateEmployeeDto dto)
    {
        var employee = _mapper.Map<Employee>(dto);
        await _employeeService.UpdateAsync(id, employee);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _employeeService.DeleteAsync(id);
        return NoContent();
    }
}