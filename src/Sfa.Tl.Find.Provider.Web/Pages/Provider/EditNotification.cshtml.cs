using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Web.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

[Authorize(nameof(PolicyNames.HasProviderAccount))]
public class EditNotificationModel : PageModel
{
    private readonly IProviderDataService _providerDataService;
    private readonly ISessionService _sessionService;
    private readonly ILogger<EditNotificationModel> _logger;

    public int ProviderNotificationId { get; private set; }

    public bool HasAvailableLocations { get; private set; }

    public IEnumerable<NotificationLocationSummary>? NotificationLocationList { get; private set; }

    [TempData]
    public string? RemovedLocation { get; set; }

    public Notification? Notification { get; private set; }

    public EditNotificationModel(
        IProviderDataService providerDataService,
        ISessionService? sessionService,
        ILogger<EditNotificationModel> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> OnGet(
        [FromQuery(Name = "id")]
        int providerNotificationId)
    {
        ProviderNotificationId = providerNotificationId;
        Notification = await _providerDataService.GetNotification(providerNotificationId);
        if (Notification is null)
        {
            return RedirectToPage("/Provider/Notifications");
        }

        NotificationLocationList = await _providerDataService.GetNotificationLocationSummaryList(providerNotificationId);

        HasAvailableLocations = (await _providerDataService
                .GetAvailableNotificationLocationPostcodes(providerNotificationId))
                .Any();

        return Page();
    }

    public async Task<IActionResult> OnGetRemoveLocation(int id, int providerNotificationId)
    {
        var notificationLocation = await _providerDataService.GetNotificationLocation(id);

        if (notificationLocation is not null)
        {
            await _providerDataService.DeleteNotificationLocation(id);

            TempData[nameof(RemovedLocation)] =
                notificationLocation.LocationName is not null
                ? $"{notificationLocation.LocationName?.ToUpper()} [{notificationLocation.Postcode}]"
                : "All";
        }

        return RedirectToPage("/Provider/EditNotification", new { id = providerNotificationId });
    }
}