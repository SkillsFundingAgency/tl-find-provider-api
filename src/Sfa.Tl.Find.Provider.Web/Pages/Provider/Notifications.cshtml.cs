using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Web.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

[Authorize(nameof(PolicyNames.HasProviderAccount))]
public class NotificationsModel : PageModel
{
    private readonly IProviderDataService _providerDataService;
    private readonly ISessionService _sessionService;
    private readonly ProviderSettings _providerSettings;
    private readonly ILogger<NotificationsModel> _logger;

    public IEnumerable<NotificationSummary>? NotificationList { get; private set; }

    [TempData]
    public string? AddedNotificationEmail { get; set; }

    [TempData]
    public string? DeletedNotificationEmail { get; set; }

    [TempData]
    public string? VerificationEmail { get; set; }

    public NotificationsModel(
        IProviderDataService providerDataService,
        ISessionService? sessionService,
        IOptions<ProviderSettings> providerOptions,
        ILogger<NotificationsModel> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _providerSettings = providerOptions?.Value
                            ?? throw new ArgumentNullException(nameof(providerOptions));
    }

    public async Task OnGet()
    {
        var ukPrn = HttpContext.User.GetUkPrn();
        if (ukPrn is not null && ukPrn > 0)
        {
            NotificationList = await _providerDataService.GetNotificationSummaryList(ukPrn.Value);
        }
    }

    public async Task<IActionResult> OnGetResendEmailVerification(int id)
    {
        var notification = await _providerDataService.GetNotification(id);
        if (notification is null)
        {
            return NotFound();
        }

        TempData[nameof(VerificationEmail)] = notification.Email;

        await _providerDataService.SendEmailVerification(id);

        return RedirectToPage("/Provider/Notifications");
    }
}