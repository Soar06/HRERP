using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HR.Web.Pages;

public class ContactModel : PageModel
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
}