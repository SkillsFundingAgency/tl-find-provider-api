using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Web.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Pages;
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        ILogger<IndexModel> logger)
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
                    nameof(IndexModel),
                    AuthenticationExtensions.AuthenticatedUserStartPage);
            }

            return RedirectToPage(AuthenticationExtensions.AuthenticatedUserStartPageWithNoSlugs);
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Index page redirecting to start page.");
        }

        return RedirectToPage(AuthenticationExtensions.UnauthenticatedUserStartPage);
    }
}
