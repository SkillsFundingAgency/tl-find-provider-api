using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Web.Pages.Provider;

public class SearchFilterDetailsModel : PageModel
{
    private readonly IProviderDataService _providerDataService;
    private readonly ILogger<SearchFilterDetailsModel> _logger;

    public string? LocationName { get; private set; }

    public SearchFilterDetailsModel(
        IProviderDataService providerDataService,
        ILogger<SearchFilterDetailsModel> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnGet()
    {
        LocationName = "PLACEHOLDER_NAME";
    }
}