using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Constants = Sfa.Tl.Find.Provider.Application.Models.Constants;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

[Authorize(nameof(PolicyNames.HasProviderAccount))]
public class EditNotificationModel : PageModel
{
    private readonly IProviderDataService _providerDataService;
    private readonly ISessionService _sessionService;
    private readonly ProviderSettings _providerSettings;
    private readonly ILogger<EditNotificationModel> _logger;

    public IEnumerable<NotificationLocationSummary>? NotificationLocationList { get; private set; }

    public Notification? Notification { get; private set; }

    public int DefaultSearchRadius { get; private set; }

    public SelectListItem[]? SearchRadiusOptions { get; private set; }

    public SelectListItem[]? FrequencyOptions { get; private set; }

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
        NotificationLocationList = await _providerDataService.GetNotificationLocationSummaryList(id);
        
        Notification = await _providerDataService.GetNotification(id);

        if (Notification is null)
        {
            return RedirectToPage("/Error/404");
        }

        await LoadNotificationView();

        //Input ??= new InputModel();
        //Input.Id = id;

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

        await _providerDataService.SaveNotification(notification, 0);

        return RedirectToPage("/Provider/Notifications");
    }

    private async Task LoadNotificationView()
    {
        DefaultSearchRadius = _providerSettings.DefaultSearchRadius > 0
            ? _providerSettings.DefaultSearchRadius
            : Constants.DefaultProviderSearchRadius;

        Input ??= new InputModel
        {
            Id = Notification?.Id ?? 0,
            SelectedFrequency = NotificationFrequency.Immediately,
            SelectedSearchRadius = DefaultSearchRadius
                .ToString()
        };

        Input.SelectedSearchRadius = DefaultSearchRadius
            //(Notification.SearchRadius ?? DefaultSearchRadius)
            .ToString();

        //Input.SelectedFrequency = NotificationFrequency.Immediately;

        FrequencyOptions = Enum.GetValues<NotificationFrequency>()
                .Select(f => new SelectListItem
                {
                    Value = ((int)f).ToString(),
                    Text = f.ToString(),
                    Selected = f == Input.SelectedFrequency
                })
                .ToArray();

        SearchRadiusOptions =
            new List<int> { 5, 10, 20, 30, 40, 50 }
                .Select(p => new SelectListItem(
                    $"{p} miles",
                    p.ToString(),
                    p.ToString() == Input?.SelectedSearchRadius)
                )
                .OrderBy(x => int.Parse(x.Value))
                .ToArray();

        Input.SkillAreas = (await _providerDataService
                .GetRoutes())
            .Select(r => new SelectListItem(
                r.Name,
                r.Id.ToString(),
                //SearchFilter.Routes.Any(x => r.Id == x.Id))
                false)
            )
            .OrderBy(x => x.Text)
            .ToArray();
    }

    public class InputModel
    {
        public int Id { get; set; }

        public NotificationFrequency SelectedFrequency { get; set; }

        public string? SelectedSearchRadius { get; set; }

        public SelectListItem[]? SkillAreas { get; set; }
    }
}