using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Web.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

[Authorize(nameof(PolicyNames.HasProviderAccount))]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class NotificationsModel : PageModel
{
    private readonly INotificationService _notificationService;
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
        INotificationService notificationService,
        ILogger<NotificationsModel> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> OnGet(
        [FromQuery(Name="token")] string? token = null)
    {
        if (token is not null)
        {
            var (success, email) = await _notificationService
                .VerifyNotificationEmail(token);

            if (success)
            {
                TempData[nameof(VerifiedEmail)] = email;
            }

            return RedirectToPage("/Provider/Notifications");
        }

        var ukPrn = HttpContext.User.GetUkPrn();
        if (ukPrn > 0)
        {
            NotificationList = await _notificationService.GetNotificationSummaryList(ukPrn.Value);
        }

        return Page();
    }

    public async Task<IActionResult> OnGetResendEmailVerification(int id)
    {
        var notification = await _notificationService.GetNotification(id);
        if (notification is null)
        {
            return NotFound();
        }

        await _notificationService.SendProviderNotificationVerificationEmail(id, notification.Email);

        TempData[nameof(VerificationEmail)] = notification.Email;

        return RedirectToPage("/Provider/Notifications");
    }
}