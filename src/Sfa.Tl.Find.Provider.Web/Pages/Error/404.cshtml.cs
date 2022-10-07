using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sfa.Tl.Find.Provider.Web.Pages.Error;

public class Error404Model : PageModel
{
    private readonly ILogger<Error404Model> _logger;

    public Error404Model(ILogger<Error404Model> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public IActionResult OnGetContactSupportClick()
    {
        _logger.LogInformation("Error/404 contact support link clicked");
        return RedirectToPage(PageContext.ActionDescriptor.ViewEnginePath);
    }
}