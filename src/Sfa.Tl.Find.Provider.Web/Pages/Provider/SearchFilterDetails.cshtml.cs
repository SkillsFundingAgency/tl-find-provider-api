using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Constants = Sfa.Tl.Find.Provider.Application.Models.Constants;
using Route = Sfa.Tl.Find.Provider.Application.Models.Route;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

public class SearchFilterDetailsModel : PageModel
{
    private readonly IProviderDataService _providerDataService;
    private readonly ProviderSettings _providerSettings;
    private readonly ILogger<SearchFilterDetailsModel> _logger;

    public int DefaultSearchRadius { get; private set; }

    public SelectListItem[]? SearchRadiusOptions { get; private set; }

    public SearchFilter? SearchFilter { get; private set; }

    [BindProperty] public InputModel? Input { get; set; }

    public SearchFilterDetailsModel(
        IProviderDataService providerDataService,
        IOptions<ProviderSettings> providerOptions,
        ILogger<SearchFilterDetailsModel> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _providerSettings = providerOptions?.Value
                            ?? throw new ArgumentNullException(nameof(providerOptions));
    }

    public async Task<IActionResult> OnGet(int id)
    {
        DefaultSearchRadius = _providerSettings.DefaultSearchRadius > 0
            ? _providerSettings.DefaultSearchRadius
            : Constants.DefaultProviderSearchRadius;

        SearchFilter = await _providerDataService.GetSearchFilter(id);

        if (SearchFilter is null)
        {
            //Does this happen? should be loading from the location id so always succeeds?
        }

        if (SearchFilter is not null)
        {
            Input ??= new InputModel();
            Input.LocationId = id;
            Input.SelectedSearchRadius = SearchFilter.SearchRadius ?? DefaultSearchRadius;

            SearchRadiusOptions = LoadSearchRadiusOptions(Input?.SelectedSearchRadius);
            Input.SkillAreas = await LoadSkillAreaOptions(SearchFilter.Routes.ToList());
        }

        return SearchFilter != null ?
            Page() :
            RedirectToPage("/Error/404");
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
        }

        var routes = Input?.SkillAreas != null
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

        var searchFilter = new SearchFilter
        {
            LocationId = Input!.LocationId,
            SearchRadius = Input?.SelectedSearchRadius is not null 
                ? Input!.SelectedSearchRadius
                : _providerSettings.DefaultSearchRadius,
            Routes = routes
        };

        await _providerDataService.SaveSearchFilter(searchFilter);

        return RedirectToPage("/Provider/SearchFilters");
    }

    private SelectListItem[] LoadSearchRadiusOptions(int? selectedValue)
    {
        return new List<int> { 5, 10, 20, 30, 40, 50 }
            .Select(p => new SelectListItem(
                $"{p} miles",
                p.ToString(),
                p == selectedValue)
            )
            .OrderBy(x => int.Parse(x.Value))
            .ToArray();
    }

    private async Task<SelectListItem[]> LoadSkillAreaOptions(IList<Route> selectedRoutes)
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

    public class InputModel
    {
        public int LocationId { get; set; }

        public int? SelectedSearchRadius { get; set; }

        public SelectListItem[]? SkillAreas { get; set; }
    }
}