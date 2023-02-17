using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Web.Authorization;
using System.ComponentModel.DataAnnotations;
using Constants = Sfa.Tl.Find.Provider.Application.Models.Constants;
using Route = Sfa.Tl.Find.Provider.Application.Models.Route;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

[Authorize(nameof(PolicyNames.HasProviderAccount))]
public class AddNotificationLocationModel : PageModel
{
    private readonly INotificationService _notificationService;
    private readonly IProviderDataService _providerDataService;
    private readonly ProviderSettings _providerSettings;
    private readonly ILogger<AddNotificationLocationModel> _logger;

    public Notification? ProviderNotification { get; private set; }

    public SelectListItem[]? Locations { get; private set; }

    public SelectListItem[]? SearchRadiusOptions { get; private set; }

    public SelectListItem[]? FrequencyOptions { get; private set; }

    [BindProperty] public InputModel? Input { get; set; }

    public AddNotificationLocationModel(
        INotificationService notificationService,
        IProviderDataService providerDataService,
        IOptions<ProviderSettings> providerOptions,
        ILogger<AddNotificationLocationModel> logger)
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
            return RedirectToPage("/Provider/EditNotification", new { id = providerNotificationId });
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

        return RedirectToPage("/Provider/EditNotification", new { id = Input?.ProviderNotificationId });
    }
        
    private async Task Save()
    {
        var routes = GetSelectedSkillAreas(Input!.SkillAreas);

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

        FrequencyOptions = LoadFrequencyOptions(Input.SelectedFrequency);

        Locations = LoadProviderLocationOptions(providerLocations, Input.SelectedLocation);

        SearchRadiusOptions = LoadSearchRadiusOptions(Input.SelectedSearchRadius);

        var routes = GetSelectedSkillAreas(Input!.SkillAreas);
        Input.SkillAreas = await LoadSkillAreaOptions(routes);
    }

    private SelectListItem[] LoadFrequencyOptions(NotificationFrequency selectedValue)
    {
        return Enum.GetValues<NotificationFrequency>()
            .Select(f => new SelectListItem
            {
                Value = ((int)f).ToString(),
                Text = f.ToString(),
                Selected = f == selectedValue
            })
            .ToArray();
    }

    private SelectListItem[] LoadSearchRadiusOptions(int? selectedValue)
    {
        return new List<int> { 5, 10, 20, 30, 40, 50 }
            .Select(p => new SelectListItem(
                $"{p} miles",
                p.ToString(),
                p == selectedValue)
            )
            .ToArray();
    }

    private async Task<SelectListItem[]> LoadSkillAreaOptions(IEnumerable<Route> selectedRoutes)
    {
        return (await _providerDataService
                .GetRoutes())
            .Select(r => new SelectListItem(
                r.Name,
                r.Id.ToString(),
                selectedRoutes.Any(x => r.Id == x.Id))
            )
            .OrderBy(x => x.Text)
            .ToArray();
    }

    private SelectListItem[] LoadProviderLocationOptions(IList<NotificationLocationName> providerLocations, int? selectedValue)
    {
        var selectList = providerLocations
            .Select(p
                => new SelectListItem(
                    $"{p.Name.TruncateWithEllipsis(15).ToUpper()} [{p.Postcode}]",
                    p.LocationId.ToString(),
                    p.LocationId == selectedValue))
            .OrderBy(x => x.Text)
            .ToList();

        return providerLocations.Count == 1
            ? selectList.ToArray()
            : selectList
                .Prepend(new SelectListItem("Select a campus", "", true))
                .ToArray();
    }

    private IList<Route> GetSelectedSkillAreas(SelectListItem[]? selectList)
    {
        return selectList != null
            ? selectList
                .Where(s => s.Selected)
                .Select(s =>
                    new Route
                    {
                        Id = int.Parse(s.Value)
                    })
                .ToList()
            : new List<Route>();
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