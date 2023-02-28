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
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Extensions;
using Constants = Sfa.Tl.Find.Provider.Application.Models.Constants;
using Route = Sfa.Tl.Find.Provider.Application.Models.Route;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

[Authorize(nameof(PolicyNames.IsProvider))]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class AddNotificationModel : PageModel
{
    private readonly IProviderDataService _providerDataService;
    private readonly INotificationService _notificationService;
    private readonly ProviderSettings _providerSettings;
    private readonly ILogger<AddNotificationModel> _logger;

    public SelectListItem[]? Locations { get; private set; }

    public SelectListItem[]? SearchRadiusOptions { get; private set; }

    public SelectListItem[]? FrequencyOptions { get; private set; }

    [BindProperty] public InputModel? Input { get; set; }

    public AddNotificationModel(
        INotificationService notificationService,
        IProviderDataService providerDataService,
        IOptions<ProviderSettings> providerOptions,
        ILogger<AddNotificationModel> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
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

        await Save();

        TempData[nameof(NotificationsModel.VerificationEmail)] = Input!.Email;

        return RedirectToPage("/Provider/Notifications");
    }

    public async Task<IActionResult> OnPostAddLocation()
    {
        if (!ModelState.IsValid)
        {
            await LoadNotificationView();
            return Page();
        }

        var providerNotificationId = await Save();
        
        return RedirectToPage("/Provider/AddAdditionalNotification",
            new { id = providerNotificationId });
    }

    private async Task<int> Save()
    {
        var ukPrn = HttpContext.User.GetUkPrn().Value;

        var routes = SelectListHelperExtensions.GetSelectedSkillAreas(Input!.SkillAreas);

        var notification = new Notification
        {
            Email = Input.Email,
            LocationId = Input.SelectedLocation is not null && Input.SelectedLocation > 0 ? Input.SelectedLocation : null,
            Frequency = Input.SelectedFrequency,
            SearchRadius = Input.SelectedSearchRadius,
            Routes = routes
        };

        return await _notificationService.CreateNotification(notification, ukPrn);
    }

    private async Task LoadNotificationView()
    {
        var defaultNotificationSearchRadius = _providerSettings.DefaultNotificationSearchRadius > 0
            ? _providerSettings.DefaultNotificationSearchRadius
            : Constants.DefaultProviderNotificationFilterRadius;

        var ukPrn = HttpContext.User.GetUkPrn();

        var providerLocations =
            (await GetProviderLocations(ukPrn))
            .ToList();

        Input ??= new InputModel
        {
            SelectedSearchRadius = defaultNotificationSearchRadius,
            SelectedFrequency = NotificationFrequency.Immediately,
            SelectedLocation = 0
        };

        FrequencyOptions = SelectListHelperExtensions.LoadFrequencyOptions(Input.SelectedFrequency);

        if (providerLocations.Count == 1)
        {
            Input.SelectedLocation = providerLocations.Single().Id;
        }
        else
        {
            Locations = SelectListHelperExtensions.LoadProviderLocationOptionsWithAllOption(providerLocations, Input.SelectedLocation);
        }

        SearchRadiusOptions = SelectListHelperExtensions.LoadSearchRadiusOptions(Input.SelectedSearchRadius);

        Input.SkillAreas = SelectListHelperExtensions.LoadSkillAreaOptions(
            await _providerDataService.GetRoutes(),
            new List<Route>());
    }
    
    private async Task<IEnumerable<LocationPostcode>> GetProviderLocations(long? ukPrn)
    {
        if (ukPrn is null) return Array.Empty<LocationPostcode>();

        return await _providerDataService
            .GetLocationPostcodes(ukPrn.Value);
    }
    
    public class InputModel
    {
        [Required(ErrorMessage = "Enter an email")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string? Email { get; set; }

        public NotificationFrequency SelectedFrequency { get; set; }

        public int? SelectedLocation { get; set; }

        public int? SelectedSearchRadius { get; set; }

        public SelectListItem[]? SkillAreas { get; set; }
    }
}