using HR.Core.Entities;
using HR.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HR.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmployeeController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("list")]
    public async Task<IActionResult> GetEmployeeList()
    {
        var employees = await _context.Employees
            .Include(e => e.Department)
            .Select(e => new
            {
                e.Id,
                e.Name,
                e.Email,
                DepartmentName = e.Department != null ? e.Department.Name : "Not Assigned",
                e.Level,
                e.Ratings
            })
            .ToListAsync();

        return Ok(employees);
    }

    [Authorize(Roles = "Employee")]
    [HttpPost("create-profile")]
    public async Task<IActionResult> CreateProfile([FromBody] EmployeeProfileRequest request)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid user ID in token.");
        }

        var existingEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
        if (existingEmployee != null)
        {
            return BadRequest("Employee profile already exists for this user.");
        }

        var department = await _context.Departments.FindAsync(request.DepartmentId);
        if (department == null)
        {
            return BadRequest("Invalid department ID.");
        }

        var validLevels = new[] { "Fresher", "Junior", "Mid-level", "Senior" };
        if (!validLevels.Contains(request.Level))
        {
            return BadRequest("Invalid level. Must be Fresher, Junior, Mid-level, or Senior.");
        }

        var employee = new Employee
        {
            Name = request.Name,
            Email = request.Email,
            Age = request.Age,
            PhoneNumber = request.PhoneNumber,
            ContractLink = request.ContractLink,
            DepartmentId = request.DepartmentId,
            Level = request.Level,
            UserId = userId
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return Ok("Employee profile created successfully.");
    }

    [Authorize(Roles = "Employee")]
    [HttpPost("request-department-change")]
    public async Task<IActionResult> RequestDepartmentChange([FromBody] DepartmentChangeRequest request)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid user ID in token.");
        }

        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
        if (employee == null)
        {
            return BadRequest("Employee profile not found. Please create a profile first.");
        }

        var department = await _context.Departments.FindAsync(request.NewDepartmentId);
        if (department == null)
        {
            return BadRequest("Invalid department ID.");
        }

        employee.DepartmentId = request.NewDepartmentId;
        await _context.SaveChangesAsync();

        return Ok("Department change request processed successfully.");
    }
}

public class EmployeeProfileRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string ContractLink { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string Level { get; set; } = "Fresher";
}

public class DepartmentChangeRequest
{
    public int NewDepartmentId { get; set; }
}