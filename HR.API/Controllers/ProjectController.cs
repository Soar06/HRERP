using HR.Core.Entities;
using HR.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class ProjectController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet("list")]
    public async Task<IActionResult> GetProjects()
    {
        var projects = await _context.Projects
            .Include(p => p.EmployeeProjects)
            .ThenInclude(ep => ep.Employee)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Detail = p.Detail,
                DueDate = p.DueDate,
                RequiredMembers = p.RequiredMembers,
                RequiredFreshers = p.RequiredFreshers,
                RequiredJuniors = p.RequiredJuniors,
                RequiredMidLevels = p.RequiredMidLevels,
                RequiredSeniors = p.RequiredSeniors,
                AssignedEmployees = p.EmployeeProjects.Select(ep => new AssignedEmployeeDto
                {
                    EmployeeId = ep.EmployeeId,
                    Name = ep.Employee.Name,
                    Level = ep.Employee.Level
                }).ToList()
            })
            .ToListAsync();

        return Ok(projects);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
    {
        // Validate DueDate
        if (dto.DueDate <= DateTime.UtcNow)
        {
            return BadRequest("Due date must be in the future.");
        }

        // Validate required members
        int totalRequiredByLevel = dto.RequiredFreshers + dto.RequiredJuniors + dto.RequiredMidLevels + dto.RequiredSeniors;
        if (totalRequiredByLevel > dto.RequiredMembers)
        {
            return BadRequest("Total required members by level cannot exceed the total required members.");
        }

        // Create the project
        var project = new Project
        {
            Name = dto.Name,
            Detail = dto.Detail,
            DueDate = dto.DueDate,
            RequiredMembers = dto.RequiredMembers,
            RequiredFreshers = dto.RequiredFreshers,
            RequiredJuniors = dto.RequiredJuniors,
            RequiredMidLevels = dto.RequiredMidLevels,
            RequiredSeniors = dto.RequiredSeniors
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Fetch available employees who are not assigned to a project
        var employees = await _context.Employees
            .Where(e => !_context.EmployeeProjects.Any(ep => ep.EmployeeId == e.Id))
            .ToListAsync();

        // Assign employees based on level requirements
        var assignmentStatus = new ProjectAssignmentStatusDto
        {
            RequiredMembers = dto.RequiredMembers,
            RequiredFreshers = dto.RequiredFreshers,
            RequiredJuniors = dto.RequiredJuniors,
            RequiredMidLevels = dto.RequiredMidLevels,
            RequiredSeniors = dto.RequiredSeniors,
            AssignedMembers = 0,
            AssignedFreshers = 0,
            AssignedJuniors = 0,
            AssignedMidLevels = 0,
            AssignedSeniors = 0
        };

        var employeeProjects = new List<EmployeeProject>();

        // Assign Freshers
        var freshers = employees.Where(e => e.Level == "Fresher").Take(dto.RequiredFreshers).ToList();
        foreach (var fresher in freshers)
        {
            employeeProjects.Add(new EmployeeProject { EmployeeId = fresher.Id, ProjectId = project.Id });
            assignmentStatus.AssignedFreshers++;
            assignmentStatus.AssignedMembers++;
            employees.Remove(fresher); // Remove assigned employee to avoid reassignment
        }

        // Assign Juniors
        var juniors = employees.Where(e => e.Level == "Junior").Take(dto.RequiredJuniors).ToList();
        foreach (var junior in juniors)
        {
            employeeProjects.Add(new EmployeeProject { EmployeeId = junior.Id, ProjectId = project.Id });
            assignmentStatus.AssignedJuniors++;
            assignmentStatus.AssignedMembers++;
            employees.Remove(junior);
        }

        // Assign Mid-levels
        var midLevels = employees.Where(e => e.Level == "MidLevel").Take(dto.RequiredMidLevels).ToList();
        foreach (var midLevel in midLevels)
        {
            employeeProjects.Add(new EmployeeProject { EmployeeId = midLevel.Id, ProjectId = project.Id });
            assignmentStatus.AssignedMidLevels++;
            assignmentStatus.AssignedMembers++;
            employees.Remove(midLevel);
        }

        // Assign Seniors
        var seniors = employees.Where(e => e.Level == "Senior").Take(dto.RequiredSeniors).ToList();
        foreach (var senior in seniors)
        {
            employeeProjects.Add(new EmployeeProject { EmployeeId = senior.Id, ProjectId = project.Id });
            assignmentStatus.AssignedSeniors++;
            assignmentStatus.AssignedMembers++;
            employees.Remove(senior);
        }

        // Fill remaining required members with any available employees (if needed)
        int remainingMembers = dto.RequiredMembers - assignmentStatus.AssignedMembers;
        if (remainingMembers > 0)
        {
            var remainingEmployees = employees.Take(remainingMembers).ToList();
            foreach (var emp in remainingEmployees)
            {
                employeeProjects.Add(new EmployeeProject { EmployeeId = emp.Id, ProjectId = project.Id });
                assignmentStatus.AssignedMembers++;
                if (emp.Level == "Fresher") assignmentStatus.AssignedFreshers++;
                else if (emp.Level == "Junior") assignmentStatus.AssignedJuniors++;
                else if (emp.Level == "MidLevel") assignmentStatus.AssignedMidLevels++;
                else if (emp.Level == "Senior") assignmentStatus.AssignedSeniors++;
            }
        }

        // Set RequirementsMet
        assignmentStatus.RequirementsMet = assignmentStatus.AssignedFreshers >= dto.RequiredFreshers &&
                                          assignmentStatus.AssignedJuniors >= dto.RequiredJuniors &&
                                          assignmentStatus.AssignedMidLevels >= dto.RequiredMidLevels &&
                                          assignmentStatus.AssignedSeniors >= dto.RequiredSeniors &&
                                          assignmentStatus.AssignedMembers >= dto.RequiredMembers;

        // Save the EmployeeProject assignments
        if (employeeProjects.Any())
        {
            _context.EmployeeProjects.AddRange(employeeProjects);
            await _context.SaveChangesAsync();
        }

        return Ok(assignmentStatus);
    }
}

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int RequiredMembers { get; set; }
    public int RequiredFreshers { get; set; }
    public int RequiredJuniors { get; set; }
    public int RequiredMidLevels { get; set; }
    public int RequiredSeniors { get; set; }
}

public class ProjectAssignmentStatusDto
{
    public int AssignedMembers { get; set; }
    public bool RequirementsMet { get; set; }
    public int AssignedFreshers { get; set; }
    public int AssignedJuniors { get; set; }
    public int AssignedMidLevels { get; set; }
    public int AssignedSeniors { get; set; }
    public int RequiredMembers { get; set; }
    public int RequiredFreshers { get; set; }
    public int RequiredJuniors { get; set; }
    public int RequiredMidLevels { get; set; }
    public int RequiredSeniors { get; set; }
}
public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int RequiredMembers { get; set; }
    public int RequiredFreshers { get; set; }
    public int RequiredJuniors { get; set; }
    public int RequiredMidLevels { get; set; }
    public int RequiredSeniors { get; set; }
    public List<AssignedEmployeeDto> AssignedEmployees { get; set; } = new();
}

public class AssignedEmployeeDto
{
    public int EmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
}