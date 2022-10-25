using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Extensions;

namespace Sfa.Tl.Find.Provider.Web.Pages.EmployerInterest;

[Authorize(nameof(PolicyNames.EmployerInterestViewer))]
public class IndexModel : PageModel
{
    public IEnumerable<EmployerInterestSummary>? EmployerInterestList { get; private set; }

    public IEnumerable<LocationPostcode>? ProviderLocations { get; private set; }

    public int EmployerInterestRetentionDays =>
        _employerInterestService.RetentionDays;

    private IEmployerInterestService _employerInterestService;
    private IProviderDataService _providerDataService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        IEmployerInterestService employerInterestService,
        IProviderDataService providerDataService,
        ILogger<IndexModel> logger)
    {
        _employerInterestService =
            employerInterestService ?? throw new ArgumentNullException(nameof(employerInterestService));
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task OnGet()
    {
        var ukPrnClaim = HttpContext.User.GetClaim(CustomClaimTypes.UkPrn);
        if (ukPrnClaim is not null &&
            long.TryParse(ukPrnClaim, out var ukPrn))
        {
            ProviderLocations = await _providerDataService.GetLocationPostcodes(ukPrn);
            //Need to pass the currently selected location into the find, so it can get by distance
            EmployerInterestList = await _employerInterestService.FindEmployerInterest();
        }
        else
        {
            //Non-provider organisation
            EmployerInterestList = await _employerInterestService.FindEmployerInterest();
        }
    }
}