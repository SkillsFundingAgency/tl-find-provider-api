using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Web.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

[Authorize(nameof(PolicyNames.HasProviderAccount))]
public class NotificationsModel : PageModel
{
    private readonly IProviderDataService _providerDataService;
    private readonly ILogger<NotificationsModel> _logger;

    public NotificationsModel(
        IProviderDataService providerDataService,
        ILogger<NotificationsModel> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnGet()
    {
    }
}