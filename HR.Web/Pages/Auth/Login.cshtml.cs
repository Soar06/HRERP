using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HR.Web.Pages.Auth;

public class LoginModel : PageModel
{
    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;

    private readonly HttpClient _httpClient;

    public LoginModel(IHttpClientFactory httpClientFactory)
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
            var loginRequest = new { Email, Password };
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (responseContent?.Token != null)
                {
                    // Store the JWT token in a cookie
                    Response.Cookies.Append("JwtToken", responseContent.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddHours(1)
                    });

                    // Decode the JWT token to get the user's role
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtToken = tokenHandler.ReadJwtToken(responseContent.Token);
                    var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                    // Debug: Log all claims to verify
                    foreach (var claim in jwtToken.Claims)
                    {
                        System.Diagnostics.Debug.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
                    }

                    SuccessMessage = "Login successful! Redirecting...";

                    // Redirect based on role (case-insensitive comparison)
                    if (string.Equals(roleClaim, "Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToPage("/Admin/Dashboard");
                    }
                    else if (string.Equals(roleClaim, "Employee", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToPage("/Employee/Dashboard");
                    }
                    else
                    {
                        ErrorMessage = $"Unknown role: {roleClaim ?? "No role claim found"}.";
                        return Page();
                    }
                }
                else
                {
                    ErrorMessage = "Invalid response from server.";
                }
            }
            else
            {
                ErrorMessage = "Invalid email or password.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
        }

        return Page();
    }
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
}