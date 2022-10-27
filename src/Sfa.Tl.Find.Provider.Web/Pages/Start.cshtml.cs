using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Web.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Pages;

public class StartModel : PageModel
{
    private readonly ILogger<StartModel> _logger;

    public StartModel(
        ILogger<StartModel> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IActionResult OnGet()
    {
        if (User.Identity is { IsAuthenticated: true })
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("User is already signed in at {currentPage}. Redirecting to  {redirectUrl}.",
                    nameof(StartModel),
                    AuthenticationExtensions.AuthenticatedUserStartPageExact);
            }

            return RedirectToPage(AuthenticationExtensions.AuthenticatedUserStartPageExact);
        }

        return Page();
    }

}