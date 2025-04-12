using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HR.Web.Pages;

public class IndexModel : PageModel
{
    public void OnGet()
    {
        ViewData["Title"] = "Home page";
    }
}