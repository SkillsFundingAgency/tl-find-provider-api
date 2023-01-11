using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
    public SearchFilter? SearchFilter { get; private set; }

    public string? LocationName { get; private set; }

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

        //TODO: Remove this field
        LocationName = SearchFilter?.LocationName;

        return SearchFilter != null ?
            Page() :
            RedirectToPage("/Error/404");
    }
}