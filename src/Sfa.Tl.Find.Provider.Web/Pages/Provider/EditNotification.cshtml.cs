using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Web.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

[Authorize(nameof(PolicyNames.HasProviderAccount))]
public class EditNotificationModel : PageModel
{
    private readonly IProviderDataService _providerDataService;
    private readonly ISessionService _sessionService;
    private readonly ProviderSettings _providerSettings;
    private readonly ILogger<EditNotificationModel> _logger;
    
    public Notification? Notification { get; private set; }

    [BindProperty] public InputModel? Input { get; set; }

    public EditNotificationModel(
        IProviderDataService providerDataService,
        ISessionService? sessionService,
        IOptions<ProviderSettings> providerOptions,
        ILogger<EditNotificationModel> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _providerSettings = providerOptions?.Value
                            ?? throw new ArgumentNullException(nameof(providerOptions));
    }

    public async Task<IActionResult> OnGet(int id)
    {
        Notification = await _providerDataService.GetNotification(id);
        
        if(Notification is null)
        {
            return RedirectToPage("/Error/404");
        }

        Input ??= new InputModel();
        Input.Id = id;

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var notification = (Input?.Id is not null)
            ? await _providerDataService.GetNotification(Input.Id)
            : null;

        if (notification is null)
        {
            return NotFound();
        }

        //TODO: Copy details to a new notification

        await _providerDataService.SaveNotification(notification);

        return RedirectToPage("/Provider/Notifications");
    }

    public class InputModel
    {
        public int Id { get; set; }

        public string? Email { get; set; }
    }
}