using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Constants = Sfa.Tl.Find.Provider.Application.Models.Constants;
using Route = Sfa.Tl.Find.Provider.Application.Models.Route;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

public class EditNotificationLocationModel : PageModel
{
    private readonly IProviderDataService _providerDataService;
    private readonly ISessionService _sessionService;
    private readonly ProviderSettings _providerSettings;
    private readonly ILogger<EditNotificationLocationModel> _logger;
   
    public Notification? NotificationDetail { get; private set; }
    
    //public SelectListItem[]? Locations { get; private set; }

    public SelectListItem[]? SearchRadiusOptions { get; private set; }

    public SelectListItem[]? FrequencyOptions { get; private set; }

    [BindProperty] public InputModel? Input { get; set; }

    public EditNotificationLocationModel(
        IProviderDataService providerDataService,
        ISessionService? sessionService,
        IOptions<ProviderSettings> providerOptions,
        ILogger<EditNotificationLocationModel> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _providerSettings = providerOptions?.Value
                            ?? throw new ArgumentNullException(nameof(providerOptions));
    }

    public async Task<IActionResult> OnGet(int id, int providerNotificationId)
    {
        NotificationDetail = await _providerDataService.GetNotificationLocation(id);
        if (NotificationDetail is null)
        {
            return RedirectToPage("/Provider/EditNotification", new { id = providerNotificationId });
        }

        await LoadNotificationView(id, providerNotificationId);

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            await LoadNotificationView(Input!.Id, Input!.ProviderNotificationId);
            return Page();
        }

        await Save();

        //TempData[nameof(NotificationsModel.VerificationEmail)] = Input!.Email;

        return RedirectToPage("/Provider/EditNotification", new { id = Input?.ProviderNotificationId });
    }

    private async Task Save()
    {
        //var ukPrn = HttpContext.User.GetUkPrn().Value;

        var routes = GetSelectedSkillAreas(Input!.SkillAreas);

        var notification = new Notification
        {
            Id = Input.Id,
            //Email = Input.Email,
            //LocationId = Input.SelectedLocation is not null && Input.SelectedLocation > 0 ? Input.SelectedLocation : null,
            Frequency = Input.SelectedFrequency,
            SearchRadius = Input.SelectedSearchRadius ?? _providerSettings.DefaultSearchRadius,
            Routes = routes
        };

        await _providerDataService.SaveNotificationLocation(notification);
    }

    private async Task LoadNotificationView(int id, int providerNotificationId)
    {
        //var ukPrn = HttpContext.User.GetUkPrn();

        var defaultSearchRadius = _providerSettings.DefaultSearchRadius > 0
            ? _providerSettings.DefaultSearchRadius
            : Constants.DefaultProviderSearchRadius;

        Input ??= new InputModel
        {
            Id = id,
            ProviderNotificationId = providerNotificationId,
            SelectedSearchRadius = NotificationDetail?.SearchRadius ?? defaultSearchRadius,
            SelectedFrequency = NotificationDetail?.Frequency ?? NotificationFrequency.Immediately
            //SelectedLocation = NotificationDetail?.LocationId ?? 0
        };

        FrequencyOptions = LoadFrequencyOptions(Input.SelectedFrequency);

        //TODO: Only load unused locations
        //Locations = await LoadProviderLocationOptions(ukPrn, Input.SelectedLocation);

        SearchRadiusOptions = LoadSearchRadiusOptions(Input.SelectedSearchRadius);

        Input.SkillAreas = await LoadSkillAreaOptions(NotificationDetail?.Routes ?? new List<Route>());
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
            //.OrderBy(x => int.Parse(x.Value))
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

    //private async Task<SelectListItem[]> LoadProviderLocationOptions(long? ukPrn, int? selectedValue)
    //{
    //    if (ukPrn is null) return Array.Empty<SelectListItem>();

    //    var providerLocations = await _providerDataService
    //            .GetLocationPostcodes(ukPrn.Value);

    //    return providerLocations.Select(p
    //            => new SelectListItem(
    //                $"{p.Name.TruncateWithEllipsis(15).ToUpper()} [{p.Postcode}]",
    //                p.Id.ToString(),
    //                p.Id == selectedValue)
    //        )
    //        .OrderBy(x => x.Text)
    //        .Prepend(new SelectListItem("All", "0", selectedValue is null or 0))
    //        .ToArray();
    //}

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
        public int Id { get; set; }

        public int ProviderNotificationId { get; set; }

        //[Required(ErrorMessage = "Enter an email")]
        //[EmailAddress(ErrorMessage = "Enter a valid email")]
        //public string? Email { get; set; }

        public NotificationFrequency SelectedFrequency { get; set; }

        //public int? SelectedLocation { get; set; }

        public int? SelectedSearchRadius { get; set; }

        public SelectListItem[]? SkillAreas { get; set; }
    }
}