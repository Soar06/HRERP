using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace HR.Web.Pages.Admin;

public class DashboardModel : PageModel
{
    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string Role { get; set; } = "Employee";

    [BindProperty]
    public string DepartmentName { get; set; } = string.Empty;

    [BindProperty]
    public string ProjectName { get; set; } = string.Empty;

    [BindProperty]
    public string ProjectDetail { get; set; } = string.Empty;

    [BindProperty]
    public DateTime DueDate { get; set; }

    [BindProperty]
    public int RequiredMembers { get; set; }

    [BindProperty]
    public int RequiredFreshers { get; set; }

    [BindProperty]
    public int RequiredJuniors { get; set; }

    [BindProperty]
    public int RequiredMidLevels { get; set; }

    [BindProperty]
    public int RequiredSeniors { get; set; }

    public List<Department> Departments { get; set; } = new();
    public List<EmployeeDto> Employees { get; set; } = new();
    public List<ProjectDto> Projects { get; set; } = new();
    public ProjectAssignmentStatusDto? ProjectAssignmentStatus { get; set; }

    [BindProperty]
    public string ErrorMessage { get; set; } = string.Empty;

    [BindProperty]
    public string SuccessMessage { get; set; } = string.Empty;

    [BindProperty]
    public string ActiveSection { get; set; } = "welcome-section"; // Added ActiveSection property

    private readonly HttpClient _httpClient;

    public DashboardModel(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("https://localhost:7123");
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var token = Request.Cookies["JwtToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Auth/Login");
        }

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Fetch departments
        try
        {
            var deptResponse = await _httpClient.GetFromJsonAsync<List<Department>>("api/departments");
            if (deptResponse != null)
            {
                Departments = deptResponse;
            }
            else
            {
                ErrorMessage = "Failed to load departments.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading departments: {ex.Message}";
        }

        // Fetch employees
        try
        {
            var empResponse = await _httpClient.GetFromJsonAsync<List<EmployeeDto>>("api/employee/list");
            if (empResponse != null)
            {
                Employees = empResponse;
            }
            else
            {
                ErrorMessage = string.IsNullOrEmpty(ErrorMessage) ? "Failed to load employee list." : ErrorMessage + " Failed to load employee list.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = string.IsNullOrEmpty(ErrorMessage) ? $"Error loading employee list: {ex.Message}" : ErrorMessage + $" Error loading employee list: {ex.Message}";
        }

        // Fetch projects
        try
        {
            var projectResponse = await _httpClient.GetFromJsonAsync<List<ProjectDto>>("api/project/list");
            if (projectResponse != null)
            {
                Projects = projectResponse;
            }
            else
            {
                ErrorMessage = string.IsNullOrEmpty(ErrorMessage) ? "Failed to load projects." : ErrorMessage + " Failed to load projects.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = string.IsNullOrEmpty(ErrorMessage) ? $"Error loading projects: {ex.Message}" : ErrorMessage + $" Error loading projects: {ex.Message}";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostCreateEmployeeAsync()
    {
        try
        {
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                ErrorMessage = "You must be logged in as an admin to create accounts.";
                ActiveSection = "create-employee";
                return Page();
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var queryString = $"?email={Uri.EscapeDataString(Email)}&password={Uri.EscapeDataString(Password)}&role={Uri.EscapeDataString(Role)}";
            var response = await _httpClient.PostAsync($"api/auth/register{queryString}", null);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Employee account created successfully.";
                ActiveSection = "create-employee"; // Stay in the "Create Employee Account" section
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Failed to create account: {errorContent}";
                ActiveSection = "create-employee";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
            ActiveSection = "create-employee";
        }

        await OnGetAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostCreateDepartmentAsync()
    {
        try
        {
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                ErrorMessage = "You must be logged in as an admin to create departments.";
                ActiveSection = "manage-departments";
                return Page();
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var department = new { Name = DepartmentName };
            var response = await _httpClient.PostAsJsonAsync("api/departments", department);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Department created successfully.";
                ActiveSection = "manage-departments"; // Stay in the "Manage Departments" section
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Failed to create department: {errorContent}";
                ActiveSection = "manage-departments";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
            ActiveSection = "manage-departments";
        }

        await OnGetAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostCreateProjectAsync()
    {
        try
        {
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                ErrorMessage = "You must be logged in as an admin to create a project.";
                ActiveSection = "manage-projects";
                return Page();
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                Name = ProjectName,
                Detail = ProjectDetail,
                DueDate,
                RequiredMembers,
                RequiredFreshers,
                RequiredJuniors,
                RequiredMidLevels,
                RequiredSeniors
            };

            var response = await _httpClient.PostAsJsonAsync("api/project/create", request);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Project created successfully.";
                ActiveSection = "manage-projects"; // Stay in the "Manage Projects" section
                ProjectAssignmentStatus = await response.Content.ReadFromJsonAsync<ProjectAssignmentStatusDto>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Failed to create project: {errorContent}";
                ActiveSection = "manage-projects";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
            ActiveSection = "manage-projects";
        }

        await OnGetAsync();
        return Page();
    }

    public IActionResult OnPostLogout()
    {
        Response.Cookies.Delete("JwtToken");
        return RedirectToPage("/Index");
    }
}

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class EmployeeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public int? Ratings { get; set; }
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