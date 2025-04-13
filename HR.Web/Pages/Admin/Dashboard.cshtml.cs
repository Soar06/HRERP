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

    public async Task<IActionResult> OnPostCreateEmployeeAsync()
    {
        try
        {
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                ErrorMessage = "You must be logged in as an admin to create accounts.";
                return Page();
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var queryString = $"?email={Uri.EscapeDataString(Email)}&password={Uri.EscapeDataString(Password)}&role={Uri.EscapeDataString(Role)}";
            var response = await _httpClient.PostAsync($"api/auth/register{queryString}", null);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Employee account created successfully.";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Failed to create account: {errorContent}";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
        }

        await OnGetAsync(); // Reload departments
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
                return Page();
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var department = new { Name = DepartmentName };
            var response = await _httpClient.PostAsJsonAsync("api/departments", department);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Department created successfully.";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ErrorMessage = $"Failed to create department: {errorContent}";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
        }

        await OnGetAsync(); // Reload departments
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