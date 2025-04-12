using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HR.Web.Pages.Admin;

public class DashboardModel : PageModel
{
    public IActionResult OnGet()
    {
        var token = Request.Cookies["JwtToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Auth/Login");
        }

        return Page();
    }

    public IActionResult OnPostLogout()
    {
        Response.Cookies.Delete("JwtToken");
        return RedirectToPage("/Index");
    }
}