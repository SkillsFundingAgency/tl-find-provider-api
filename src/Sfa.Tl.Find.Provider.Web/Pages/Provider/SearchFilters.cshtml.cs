using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Constants = Sfa.Tl.Find.Provider.Application.Models.Constants;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

[Authorize(nameof(PolicyNames.HasProviderAccount))]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class SearchFiltersModel : PageModel
{
    private readonly ISearchFilterService _searchFilterService;
    private readonly ProviderSettings _providerSettings;
    private readonly ILogger<SearchFiltersModel> _logger;

    public int DefaultSearchRadius { get; private set; }
    public IEnumerable<SearchFilter>? SearchFilterList { get; private set; }

    public SearchFiltersModel(
        ISearchFilterService searchFilterService,
        IOptions<ProviderSettings> providerOptions,
        ILogger<SearchFiltersModel> logger)
    {
        _searchFilterService = searchFilterService?? throw new ArgumentNullException(nameof(searchFilterService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _providerSettings = providerOptions?.Value
                            ?? throw new ArgumentNullException(nameof(providerOptions));
    }

    public async Task OnGet()
    {
        var ukPrn = HttpContext.User.GetUkPrn();
        if (ukPrn is not null && ukPrn > 0)
        {
            SearchFilterList = await _searchFilterService.GetSearchFilterSummaryList(ukPrn.Value);
        }

        DefaultSearchRadius = _providerSettings.DefaultSearchRadius > 0
            ? _providerSettings.DefaultSearchRadius
            : Constants.DefaultProviderSearchFilterRadius;
    }
}