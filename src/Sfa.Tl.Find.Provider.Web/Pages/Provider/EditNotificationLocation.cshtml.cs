using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Constants = Sfa.Tl.Find.Provider.Application.Models.Constants;
using Route = Sfa.Tl.Find.Provider.Application.Models.Route;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

[Authorize(nameof(PolicyNames.HasProviderAccount))]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class EditNotificationLocationModel : PageModel
{
    private readonly IProviderDataService _providerDataService;
    private readonly INotificationService _notificationService;
    private readonly ProviderSettings _providerSettings;
    private readonly ILogger<EditNotificationLocationModel> _logger;

    public Notification? NotificationDetail { get; private set; }

    public SelectListItem[]? SearchRadiusOptions { get; private set; }

    public SelectListItem[]? FrequencyOptions { get; private set; }

    [BindProperty] public InputModel? Input { get; set; }

    public EditNotificationLocationModel(
        INotificationService notificationService,
        IProviderDataService providerDataService,
        IOptions<ProviderSettings> providerOptions,
        ILogger<EditNotificationLocationModel> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _providerSettings = providerOptions?.Value
                            ?? throw new ArgumentNullException(nameof(providerOptions));
    }

    public async Task<IActionResult> OnGet(
        [FromQuery(Name = "id")]
        int providerNotificationId,
        // ReSharper disable once StringLiteralTypo
        [FromQuery(Name = "locationid")]
        int notificationLocationId)
    {
        NotificationDetail = await _notificationService.GetNotificationLocation(notificationLocationId);
        if (NotificationDetail is null)
        {
            return RedirectToPage("/Provider/EditNotification", new { id = providerNotificationId });
        }

        await LoadNotificationView(providerNotificationId, notificationLocationId);

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            await LoadNotificationView(Input!.ProviderNotificationId, Input!.Id);
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
            Id = Input.Id,
            Frequency = Input.SelectedFrequency,
            SearchRadius = Input.SelectedSearchRadius ?? _providerSettings.DefaultSearchRadius,
            Routes = routes
        };

        await _notificationService.UpdateNotificationLocation(notification);
    }

    private async Task LoadNotificationView(int providerNotificationId, int id)
    {
        var defaultNotificationSearchRadius = _providerSettings.DefaultNotificationSearchRadius > 0
            ? _providerSettings.DefaultNotificationSearchRadius
            : Constants.DefaultProviderNotificationFilterRadius;

        Input ??= new InputModel
        {
            Id = id,
            ProviderNotificationId = providerNotificationId,
            SelectedSearchRadius = NotificationDetail?.SearchRadius ?? defaultNotificationSearchRadius,
            SelectedFrequency = NotificationDetail?.Frequency ?? NotificationFrequency.Immediately
        };

        FrequencyOptions = LoadFrequencyOptions(Input.SelectedFrequency);

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

        public NotificationFrequency SelectedFrequency { get; set; }

        public int? SelectedSearchRadius { get; set; }

        public SelectListItem[]? SkillAreas { get; set; }
    }
}