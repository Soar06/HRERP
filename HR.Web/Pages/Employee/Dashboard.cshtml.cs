using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HR.Web.Pages.Employee;

public class DashboardModel : PageModel
{
    public string UserEmail { get; set; } = string.Empty;

    [BindProperty]
    public string Name { get; set; } = string.Empty;

    [BindProperty]
    public string Phone { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        var token = Request.Cookies["JwtToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Auth/Login");
        }

        // In a real app, you'd fetch the user's email from the JWT token
        UserEmail = "employee@hr.com"; // Hardcoded for now
        return Page();
    }

    public IActionResult OnPostUpdateInfo()
    {
        // In a real app, you'd call an API endpoint to save the personal info
        // For now, just redirect back to the dashboard
        return RedirectToPage("/Employee/Dashboard");
    }

    public IActionResult OnPostLogout()
    {
        Response.Cookies.Delete("JwtToken");
        return RedirectToPage("/Index");
    }
}