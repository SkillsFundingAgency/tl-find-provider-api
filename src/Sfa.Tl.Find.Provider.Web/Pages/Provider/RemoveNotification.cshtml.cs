using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Sfa.Tl.Find.Provider.Web.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

[Authorize(nameof(PolicyNames.HasProviderAccount))]
public class RemoveNotificationModel : PageModel
{
    private readonly IProviderDataService _providerDataService;
    private readonly ILogger<RemoveNotificationModel> _logger;

    public string? NotificationEmail { get; private set; }

    public RemoveNotificationModel(
        IProviderDataService providerDataService,
        ILogger<RemoveNotificationModel> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnGet()
    {
        NotificationEmail = "test@education.gov.uk";
    }
}