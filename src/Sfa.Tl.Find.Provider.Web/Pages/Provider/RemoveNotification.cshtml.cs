using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Application.Models;

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
        if (id == null)
        {
            return NotFound();
        }

        Notification = await _providerDataService.GetNotification(id.Value);

        TempData["DeletedNotificationEmail"] = Notification?.Email;

        await _providerDataService.DeleteNotification(id.Value);

        return RedirectToPage("/Provider/Notifications");
    }
}