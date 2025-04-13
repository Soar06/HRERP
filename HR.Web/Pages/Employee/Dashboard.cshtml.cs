using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;

namespace HR.Web.Pages.Employee;

public class DashboardModel : PageModel
{
    public string UserEmail { get; set; } = string.Empty;

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Name { get; set; } = string.Empty;

    [BindProperty]
    public string PhoneNumber { get; set; } = string.Empty;

    [BindProperty]
    public int Age { get; set; }

    [BindProperty]
    public string ContractLink { get; set; } = string.Empty;

    [BindProperty]
    public int DepartmentId { get; set; }

    [BindProperty]
    public string Level { get; set; } = "Fresher";

    [BindProperty]
    public int NewDepartmentId { get; set; }

    public List<Department> Departments { get; set; } = new();

    public string ErrorMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;

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

        // Extract the user's email from the token
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        UserEmail = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "Unknown User";
        Email = UserEmail; // Pre-fill the email for the form

        // Add the Authorization header with the JWT token
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Fetch departments
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Department>>("api/departments");
            if (response != null)
            {
                Departments = response;
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

        return Page();
    }

    public async Task<IActionResult> OnPostUpdateInfoAsync()
    {
        try
        {
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                ErrorMessage = "You must be logged in to update your information.";
                return Page();
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                Email,
                Name,
                PhoneNumber,
                Age,
                ContractLink,
                DepartmentId,
                Level
            };

            var response = await _httpClient.PostAsJsonAsync("api/employee/create-profile", request);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Information updated and employee profile created successfully.";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Failed to update information: {errorContent}";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
        }

        await OnGetAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostRequestDepartmentChangeAsync()
    {
        try
        {
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                ErrorMessage = "You must be logged in to request a department change.";
                return Page();
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new { NewDepartmentId };
            var response = await _httpClient.PostAsJsonAsync("api/employee/request-department-change", request);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Department change request processed successfully.";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Failed to request department change: {errorContent}";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
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