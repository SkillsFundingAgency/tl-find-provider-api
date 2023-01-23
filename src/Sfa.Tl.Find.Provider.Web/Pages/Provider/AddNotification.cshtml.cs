using System.ComponentModel.DataAnnotations;
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
using Route = Sfa.Tl.Find.Provider.Application.Models.Route;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

[Authorize(nameof(PolicyNames.HasProviderAccount))]
public class AddNotificationModel : PageModel
{
    private readonly IProviderDataService _providerDataService;
    private readonly ISessionService _sessionService;
    private readonly ProviderSettings _providerSettings;
    private readonly ILogger<AddNotificationModel> _logger;

    public int DefaultSearchRadius { get; private set; }

    public SelectListItem[]? SearchRadiusOptions { get; private set; }

    public SelectListItem[]? FrequencyOptions { get; private set; }

    [BindProperty] public InputModel? Input { get; set; }

    public AddNotificationModel(
        IProviderDataService providerDataService,
        ISessionService? sessionService,
        IOptions<ProviderSettings> providerOptions,
        ILogger<AddNotificationModel> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _providerSettings = providerOptions?.Value
                            ?? throw new ArgumentNullException(nameof(providerOptions));
    }

    public async Task OnGet()
    {
        await LoadNotificationView();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            await LoadNotificationView();
            return Page();
        }

        var routes = Input!.SkillAreas != null
            ? Input
                .SkillAreas
                .Where(s => s.Selected)
                .Select(s =>
                    new Route
                    {
                        Id = int.Parse(s.Value)
                    })
                .ToList()
            : new List<Route>();

        //TODO: Campus dropdown
        var locationId = 2;

        var notification = new Notification
        {
            Email = Input.Email,
            LocationId = locationId,
            Frequency = Input.SelectedFrequency,
            SearchRadius = Input.SelectedSearchRadius is not null ?
                int.Parse(Input.SelectedSearchRadius)
                : _providerSettings.DefaultSearchRadius,
            Routes = routes
        };

        await _providerDataService.SaveNotification(notification);

        TempData[nameof(NotificationsModel.AddedNotificationEmail)] = notification.Email;

        return RedirectToPage("/Provider/Notifications");
    }

    private async Task LoadNotificationView()
    {
        DefaultSearchRadius = _providerSettings.DefaultSearchRadius > 0
            ? _providerSettings.DefaultSearchRadius
            : Constants.DefaultProviderSearchRadius;

        Input ??= new InputModel();
        //Input.NotificationId = id;
        Input.SelectedSearchRadius = DefaultSearchRadius
            //(Notification.SearchRadius ?? DefaultSearchRadius)
            .ToString();

        Input.SelectedFrequency = NotificationFrequency.Immediately;

        //var f = Enum.GetValues<NotificationFrequency>()
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
        [Required(ErrorMessage = "Enter an email")]
        public string? Email { get; set; }

        public NotificationFrequency SelectedFrequency { get; set; }

        public string? SelectedSearchRadius { get; set; }

        public SelectListItem[]? SkillAreas { get; set; }
    }
}