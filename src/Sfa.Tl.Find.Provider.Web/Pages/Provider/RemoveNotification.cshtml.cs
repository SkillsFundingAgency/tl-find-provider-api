using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Web.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

[Authorize(nameof(PolicyNames.HasProviderAccount))]
public class RemoveNotificationModel : PageModel
{
    private readonly IProviderDataService _providerDataService;
    private readonly ILogger<RemoveNotificationModel> _logger;

    public Notification? Notification { get; private set; }

    public RemoveNotificationModel(
        IProviderDataService providerDataService,
        ILogger<RemoveNotificationModel> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> OnGet(int id)
    {
        Notification = await _providerDataService.GetNotification(id);

        return Notification != null ?
            Page() :
            RedirectToPage("/Error/404");
    }

    public async Task<IActionResult> OnPost(int? id)
    {
        var notification = (id is not null)
            ? await _providerDataService.GetNotification(id.Value)
            : null;

        if (notification is null)
        {
            return NotFound();
        }

        await _providerDataService.DeleteNotification(id!.Value);

        TempData[nameof(NotificationsModel.DeletedNotificationEmail)] = notification.Email;

        return RedirectToPage("/Provider/Notifications");
    }
}