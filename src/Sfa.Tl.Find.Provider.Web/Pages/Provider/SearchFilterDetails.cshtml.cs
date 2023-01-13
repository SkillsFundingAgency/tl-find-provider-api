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

            Input.SkillAreas = (await _providerDataService
                .GetRoutes())
                .Select(r => new SelectListItem(
                    r.Name,
                    r.Id.ToString(),
                    SearchFilter.Routes.Any(x => r.Id == x.Id))
                )
                .OrderBy(x => x.Text)
                .ToArray();
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

        Debug.WriteLine($"SelectedSearchRadius = {Input?.SelectedSearchRadius}");
        if (Input.SkillAreas is not null)
        {
            foreach (var s in Input.SkillAreas)
            {
                Debug.WriteLine($"Skill area {s.Value} - {s.Selected} - {s.Text}");
            }
        }

        return RedirectToPage("/Provider/SearchFilters");
    }

    public class InputModel
    {
        public string? SelectedSearchRadius { get; set; }

        public SelectListItem[]? SkillAreas { get; set; }
    }
}