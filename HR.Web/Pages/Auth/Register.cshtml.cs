using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace HR.Web.Pages.Auth;

public class RegisterModel : PageModel
{
    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string Role { get; set; } = "Employee";

    public string ErrorMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;

    private readonly HttpClient _httpClient;

    public RegisterModel(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("https://localhost:7123");
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                ErrorMessage = "You must be logged in as an admin to register a new user.";
                return Page();
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var registerRequest = new { Email, Password, Role };
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerRequest);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "User registered successfully!";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ErrorMessage = "You must be an admin to register a new user.";
            }
            else
            {
                ErrorMessage = "Failed to register user.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
        }

        return Page();
    }
}