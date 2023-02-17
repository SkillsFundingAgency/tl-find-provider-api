using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Web.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

[Authorize(nameof(PolicyNames.HasProviderAccount))]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class EditNotificationModel : PageModel
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<EditNotificationModel> _logger;

    public int ProviderNotificationId { get; private set; }

    public bool HasAvailableLocations { get; private set; }

    public IEnumerable<NotificationLocationSummary>? NotificationLocationList { get; private set; }

    [TempData]
    public string? RemovedLocation { get; set; }

    public Notification? Notification { get; private set; }

    public EditNotificationModel(
        INotificationService notificationService,
        ILogger<EditNotificationModel> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> OnGet(
        [FromQuery(Name = "id")]
        int providerNotificationId)
    {
        ProviderNotificationId = providerNotificationId;
        Notification = await _notificationService.GetNotification(providerNotificationId);
        if (Notification is null)
        {
            return RedirectToPage("/Provider/Notifications");
        }

        NotificationLocationList = await _notificationService.GetNotificationLocationSummaryList(providerNotificationId);

        HasAvailableLocations = (await _notificationService
                .GetAvailableNotificationLocationPostcodes(providerNotificationId))
                .Any();

        return Page();
    }

    public async Task<IActionResult> OnGetRemoveLocation(int id, int providerNotificationId)
    {
        var notificationLocation = await _notificationService.GetNotificationLocation(id);

        if (notificationLocation is not null)
        {
            await _notificationService.DeleteNotificationLocation(id);

            TempData[nameof(RemovedLocation)] =
                notificationLocation.LocationName is not null
                ? $"{notificationLocation.LocationName?.ToUpper()} [{notificationLocation.Postcode}]"
                : "All";
        }

        return RedirectToPage("/Provider/EditNotification", new { id = providerNotificationId });
    }
}