using System.ComponentModel.DataAnnotations;
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
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
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

    public SelectListItem[]? Locations { get; private set; }

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

        var routes = GetSelectedSkillAreas(Input!.SkillAreas);

        var notification = new Notification
        {
            Email = Input.Email,
            LocationId = Input.SelectedLocation is not null && Input.SelectedLocation > 0 ? Input.SelectedLocation : null,
            Frequency = Input.SelectedFrequency,
            SearchRadius = Input.SelectedSearchRadius,
            Routes = routes
        };

        return await _providerDataService.SaveNotification(notification, ukPrn);
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

        FrequencyOptions = LoadFrequencyOptions(Input.SelectedFrequency);

        if (providerLocations.Count == 1)
        {
            Input.SelectedLocation = providerLocations.Single().Id;
        }
        else
        {
            Locations = LoadProviderLocationOptions(providerLocations, Input.SelectedLocation);
        }

        SearchRadiusOptions = LoadSearchRadiusOptions(Input.SelectedSearchRadius);

        Input.SkillAreas = await LoadSkillAreaOptions(new List<Route>());
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
        var values = new List<int> { 5, 10, 20, 30, 40, 50 };
        return values
            .Select(p => new SelectListItem(
                $"{p} miles",
                p.ToString(),
                p == (selectedValue ?? values[0]))
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

    private async Task<IEnumerable<LocationPostcode>> GetProviderLocations(long? ukPrn)
    {
        if (ukPrn is null) return Array.Empty<LocationPostcode>();

        return await _providerDataService
            .GetLocationPostcodes(ukPrn.Value);
    }

    private SelectListItem[] LoadProviderLocationOptions(IEnumerable<LocationPostcode> providerLocations, int? selectedValue)
    {
        return providerLocations.Select(p
                => new SelectListItem(
                    $"{p.Name.TruncateWithEllipsis(15).ToUpper()} [{p.Postcode}]",
                    p.Id.ToString(),
                    p.Id == selectedValue)
            )
            .OrderBy(x => x.Text)
            .Prepend(new SelectListItem("All", "0", selectedValue is null or 0))
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
        [Required(ErrorMessage = "Enter an email")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string? Email { get; set; }

        public NotificationFrequency SelectedFrequency { get; set; }

        public int? SelectedLocation { get; set; }

        public int? SelectedSearchRadius { get; set; }

        public SelectListItem[]? SkillAreas { get; set; }
    }
}