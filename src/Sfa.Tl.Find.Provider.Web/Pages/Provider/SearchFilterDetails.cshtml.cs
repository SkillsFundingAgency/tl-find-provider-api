using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Extensions;
using Constants = Sfa.Tl.Find.Provider.Application.Models.Constants;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

[Authorize(nameof(PolicyNames.IsProvider))]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class SearchFilterDetailsModel : PageModel
{
    private readonly IProviderDataService _providerDataService;
    private readonly ISearchFilterService _searchFilterService;
    private readonly ProviderSettings _providerSettings;
    private readonly ILogger<SearchFilterDetailsModel> _logger;

    public int DefaultSearchRadius { get; private set; }

    public SelectListItem[]? SearchRadiusOptions { get; private set; }

    public SearchFilter? SearchFilter { get; private set; }

    [BindProperty] public InputModel? Input { get; set; }

    public SearchFilterDetailsModel(
        IProviderDataService providerDataService,
        ISearchFilterService searchFilterService,
        IOptions<ProviderSettings> providerOptions,
        ILogger<SearchFilterDetailsModel> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _searchFilterService = searchFilterService ?? throw new ArgumentNullException(nameof(searchFilterService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _providerSettings = providerOptions?.Value
                            ?? throw new ArgumentNullException(nameof(providerOptions));
    }

    public async Task<IActionResult> OnGet(int id)
    {
        DefaultSearchRadius = _providerSettings.DefaultSearchRadius > 0
            ? _providerSettings.DefaultSearchRadius
            : Constants.DefaultProviderSearchFilterRadius;

        SearchFilter = await _searchFilterService.GetSearchFilter(id);

        if (SearchFilter is null)
        {
            //Does this happen? should be loading from the location id so always succeeds?
        }

        if (SearchFilter is not null)
        {
            Input ??= new InputModel();
            Input.LocationId = id;
            Input.SelectedSearchRadius = SearchFilter.SearchRadius ?? DefaultSearchRadius;

            SearchRadiusOptions = SelectListHelperExtensions.LoadSearchRadiusOptions(Input?.SelectedSearchRadius);
            Input.SkillAreas = SelectListHelperExtensions.LoadSkillAreaOptions(
                await _providerDataService.GetRoutes(),
                SearchFilter.Routes);
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

        var routes = SelectListHelperExtensions.GetSelectedSkillAreas(Input?.SkillAreas);

        var searchFilter = new SearchFilter
        {
            LocationId = Input!.LocationId,
            SearchRadius = Input?.SelectedSearchRadius is not null
                ? Input!.SelectedSearchRadius
                : _providerSettings.DefaultSearchRadius,
            Routes = routes
        };

        await _searchFilterService.SaveSearchFilter(searchFilter);

        return RedirectToPage("/Provider/SearchFilters");
    }

    public class InputModel
    {
        public int LocationId { get; set; }

        public int? SelectedSearchRadius { get; set; }

        public SelectListItem[]? SkillAreas { get; set; }
    }
}