using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Extensions;

namespace Sfa.Tl.Find.Provider.Web.Pages.EmployerInterest;

[Authorize(nameof(PolicyNames.HasProviderAccount))]
public class IndexModel : PageModel
{
    public IEnumerable<Application.Models.EmployerInterest>? EmployerInterestList { get; private set; }
    //public IEnumerable<Application.Models.ProviderLocation>? ProviderLocations { get; private set; }
    
    private IEmployerInterestService _employerInterestService;
    private IProviderDataService _providerDataService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        IEmployerInterestService employerInterestService,
        IProviderDataService providerDataService,
        ILogger<IndexModel> logger)
    {
        _employerInterestService = employerInterestService ?? throw new ArgumentNullException(nameof(employerInterestService));
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task OnGet()
    {
        var ukPrn = HttpContext.User.GetClaim(CustomClaimTypes.UkPrn);
        if (ukPrn is null)
        {
            //Something went wrong
        }
        //call provider data to get details - location postcodes and lat/long
        //var ProviderLocations = await _providerDataService.GetLocations();
        //Need to pass the currently selected location into the find, so it can get by distance
        EmployerInterestList = await _employerInterestService.FindEmployerInterest();
    }
}