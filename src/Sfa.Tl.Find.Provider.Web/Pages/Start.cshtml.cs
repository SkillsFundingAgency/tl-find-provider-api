using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Web.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Pages;

public class StartModel : PageModel
{
    private readonly ProviderSettings _providerSettings;
    private readonly ILogger<StartModel> _logger;

    public string? SupportSiteAccessConnectHelpUri { get; private set; }

    public StartModel(
        IOptions<ProviderSettings> providerOptions,
        ILogger<StartModel> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _providerSettings = providerOptions?.Value
                            ?? throw new ArgumentNullException(nameof(providerOptions));
    }

    public IActionResult OnGet()
    {
        if (User.Identity is { IsAuthenticated: true })
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("User is already signed in at {currentPage}. Redirecting to  {redirectUrl}.",
                    nameof(StartModel),
                    AuthenticationExtensions.AuthenticatedUserStartPage);
            }

            return RedirectToPage(AuthenticationExtensions.AuthenticatedUserStartPage);
        }

        SupportSiteAccessConnectHelpUri = _providerSettings.SupportSiteAccessConnectHelpUri;

        return Page();
    }
}