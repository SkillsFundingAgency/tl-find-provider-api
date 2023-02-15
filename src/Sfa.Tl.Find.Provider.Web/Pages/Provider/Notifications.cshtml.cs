using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Web.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

[Authorize(nameof(PolicyNames.HasProviderAccount))]
public class NotificationsModel : PageModel
{
    private readonly IProviderDataService _providerDataService;
    private readonly ILogger<NotificationsModel> _logger;

    public IEnumerable<NotificationSummary>? NotificationList { get; private set; }

    [TempData]
    public string? AddedNotificationEmail { get; set; }

    [TempData]
    public string? DeletedNotificationEmail { get; set; }

    [TempData]
    public string? VerificationEmail { get; set; }

    [TempData]
    public string? VerifiedEmail { get; set; }

    public NotificationsModel(
        IProviderDataService providerDataService,
        ILogger<NotificationsModel> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> OnGet(
        [FromQuery(Name="token")] string? token = null)
    {
        if (token is not null)
        {
            var verificationResult = await _providerDataService
                .VerifyNotificationEmail(token);

            if (verificationResult.Success)
            {
                TempData[nameof(VerifiedEmail)] = verificationResult.Email;
            }

            return RedirectToPage("/Provider/Notifications");
        }

        var ukPrn = HttpContext.User.GetUkPrn();
        if (ukPrn > 0)
        {
            NotificationList = await _providerDataService.GetNotificationSummaryList(ukPrn.Value);
        }

        return Page();
    }

    public async Task<IActionResult> OnGetResendEmailVerification(int id)
    {
        var notification = await _providerDataService.GetNotification(id);
        if (notification is null)
        {
            return NotFound();
        }

        await _providerDataService.SendProviderVerificationEmail(id, notification.Email);

        TempData[nameof(VerificationEmail)] = notification.Email;

        return RedirectToPage("/Provider/Notifications");
    }
}