using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Constants = Sfa.Tl.Find.Provider.Application.Models.Constants;

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
            Input.SelectedSearchRadius =
                (SearchFilter.SearchRadius ?? DefaultSearchRadius)
                .ToString();

            SearchRadiusOptions = 
                new List<int> { 5, 10, 20, 30, 40, 50 }
                    .Select(p => new SelectListItem(
                        $"{p} miles",
                        p.ToString(),
                p.ToString() == Input?.SelectedSearchRadius)
                )
                .OrderBy(x => int.Parse(x.Value))
                .ToArray();

            var skillAreas = await _providerDataService.GetRoutes();
        }

        return SearchFilter != null ?
            Page() :
            RedirectToPage("/Error/404");
    }

    public async Task<IActionResult> OnPost()
    {
        if (ModelState.IsValid)
        {
            Debug.WriteLine($"SelectedSearchRadius = {Input?.SelectedSearchRadius}");
        }

        return RedirectToPage("/Provider/SearchFilters");
    }

    public class InputModel
    {
        public string? SelectedSearchRadius { get; set; }
    }
}