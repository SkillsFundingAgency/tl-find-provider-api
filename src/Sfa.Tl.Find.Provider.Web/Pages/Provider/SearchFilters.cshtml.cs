using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Web.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

[Authorize(nameof(PolicyNames.HasProviderAccount))]
public class SearchFiltersModel : PageModel
{
    private readonly IProviderDataService _providerDataService;
    private readonly ProviderSettings _providerSettings;
    private readonly ILogger<SearchFiltersModel> _logger;

    public IEnumerable<SearchFilter>? SearchFilterList { get; private set; }

    public SearchFiltersModel(
        IProviderDataService providerDataService,
        IOptions<ProviderSettings> providerOptions,
        ILogger<SearchFiltersModel> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _providerSettings = providerOptions?.Value
                            ?? throw new ArgumentNullException(nameof(providerOptions));
    }

    public async Task OnGet()
    {
        var ukPrn = GetUkPrn();
        if (ukPrn is not null)
        {
            SearchFilterList = await _providerDataService.GetSearchFilters(ukPrn.Value);
        }
    }

    private long? GetUkPrn()
    {
        var ukPrnClaim = HttpContext.User.GetClaim(CustomClaimTypes.UkPrn);
        return ukPrnClaim is not null && long.TryParse(ukPrnClaim, out var ukPrn)
            ? ukPrn
            : null;
    }
}