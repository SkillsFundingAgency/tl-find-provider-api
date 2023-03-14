using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Constants = Sfa.Tl.Find.Provider.Application.Models.Constants;
using Sfa.Tl.Find.Provider.Web.Extensions;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

[Authorize(nameof(PolicyNames.IsProvider))]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class AddAdditionalNotificationModel : PageModel
{
    private readonly INotificationService _notificationService;
    private readonly IProviderDataService _providerDataService;
    private readonly ProviderSettings _providerSettings;
    private readonly ILogger<AddAdditionalNotificationModel> _logger;

    public Notification? ProviderNotification { get; private set; }

    public SelectListItem[]? Locations { get; private set; }

    public SelectListItem[]? SearchRadiusOptions { get; private set; }

    public SelectListItem[]? FrequencyOptions { get; private set; }

    [BindProperty] public InputModel? Input { get; set; }

    public AddAdditionalNotificationModel(
        INotificationService notificationService,
        IProviderDataService providerDataService,
        IOptions<ProviderSettings> providerOptions,
        ILogger<AddAdditionalNotificationModel> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _providerSettings = providerOptions?.Value
                            ?? throw new ArgumentNullException(nameof(providerOptions));
    }

    public async Task<IActionResult> OnGet(
            [FromQuery(Name = "id")]
        int providerNotificationId)
    {
        ProviderNotification = await _notificationService.GetNotification(providerNotificationId);
        if (ProviderNotification is null)
        {
            return RedirectToPage("/Provider/Notifications", new { id = providerNotificationId });
        }

        await LoadNotificationView(providerNotificationId);

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            await LoadNotificationView(Input!.ProviderNotificationId);
            return Page();
        }

        await Save();

        var providerNotification = await _notificationService.GetNotification(Input!.ProviderNotificationId);

        TempData[nameof(NotificationsModel.VerificationEmail)] = providerNotification?.Email;

        return RedirectToPage("/Provider/Notifications");
    }

    public async Task<IActionResult> OnPostAddLocation()
    {
        if (!ModelState.IsValid)
        {
            await LoadNotificationView(Input!.ProviderNotificationId);
            return Page();
        }

        await Save();

        return RedirectToPage("/Provider/AddAdditionalNotification",
            new
            {
                id = Input?.ProviderNotificationId
            });
    }

    private async Task Save()
    {
        var routes = SelectListHelperExtensions.GetSelectedSkillAreas(Input!.SkillAreas);

        var notification = new Notification
        {
            LocationId = Input.SelectedLocation is not null && Input.SelectedLocation > 0 ? Input.SelectedLocation : null,
            Frequency = Input.SelectedFrequency,
            SearchRadius = Input.SelectedSearchRadius,
            Routes = routes
        };

        await _notificationService.CreateNotificationLocation(notification, Input.ProviderNotificationId);
    }

    private async Task LoadNotificationView(int providerNotificationId)
    {
        var defaultNotificationSearchRadius = _providerSettings.DefaultNotificationSearchRadius > 0
            ? _providerSettings.DefaultNotificationSearchRadius
            : Constants.DefaultProviderNotificationFilterRadius;

        var providerLocations = (await _notificationService
                .GetAvailableNotificationLocationPostcodes(providerNotificationId))
            .ToList();

        Input ??= new InputModel
        {
            ProviderNotificationId = providerNotificationId,
            SelectedSearchRadius = defaultNotificationSearchRadius,
            SelectedFrequency = NotificationFrequency.Immediately,
            SelectedLocation = providerLocations.Count == 1
                ? providerLocations.Single().LocationId
                : null
        };

        FrequencyOptions = SelectListHelperExtensions.LoadFrequencyOptions(Input.SelectedFrequency);

        Locations = SelectListHelperExtensions.LoadProviderLocationOptions(providerLocations, Input.SelectedLocation);

        SearchRadiusOptions = SelectListHelperExtensions.LoadSearchRadiusOptions(Input.SelectedSearchRadius);

        Input.SkillAreas = SelectListHelperExtensions.LoadSkillAreaOptions(
            await _providerDataService.GetRoutes(),
            SelectListHelperExtensions.GetSelectedSkillAreas(Input!.SkillAreas));
    }
    
    public class InputModel
    {
        public int ProviderNotificationId { get; set; }

        public NotificationFrequency SelectedFrequency { get; set; }

        [Required(ErrorMessage = "Select a campus")]
        public int? SelectedLocation { get; set; }

        public int? SelectedSearchRadius { get; set; }

        public SelectListItem[]? SkillAreas { get; set; }
    }
}
