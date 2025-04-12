using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;

namespace HR.Web.Pages;

public class OldDashboardModel : PageModel
{
    public IActionResult OnGet()
    {
        var token = Request.Cookies["JwtToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Auth/Login");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

        if (roleClaim == "Admin")
        {
            return RedirectToPage("/Admin/Dashboard");
        }
        else if (roleClaim == "Employee")
        {
            return RedirectToPage("/Employee/Dashboard");
        }

        return RedirectToPage("/Auth/Login");
    }

    public IActionResult OnPostLogout()
    {
        Response.Cookies.Delete("JwtToken");
        return RedirectToPage("/Index");
    }
}