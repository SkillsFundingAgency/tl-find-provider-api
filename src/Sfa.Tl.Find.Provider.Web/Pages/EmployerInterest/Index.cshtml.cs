using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public bool IsProvider => UkPrn.HasValue;

    public long? UkPrn { get; private set; }

    [BindProperty]
    public string? Postcode { get; set; }

    //public string Name { get; set; }

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
        GetValuesFromClaims();
        
        if (UkPrn.HasValue)
        {
            await LoadProviderPostcodes(UkPrn.Value);
            EmployerInterestList = await _employerInterestService.GetSummaryList();
        }
        else
        {
            //Non-provider organisation
            EmployerInterestList = await _employerInterestService.GetSummaryList();
        }
    }

    public async Task OnPost()
    {
        GetValuesFromClaims();
        if (UkPrn.HasValue)
        {
            await LoadProviderPostcodes(UkPrn.Value);
        }

        if (!ModelState.IsValid)
        {
            return;// Page();
        }

        //Will need to reload providers as well - 
        if (!string.IsNullOrEmpty(Postcode))
        {
            EmployerInterestList = await _employerInterestService.FindEmployerInterest(Postcode);
        }

        //return Page();
    }

    private void GetValuesFromClaims()
    {
        var ukPrnClaim = HttpContext.User.GetClaim(CustomClaimTypes.UkPrn);
        UkPrn = ukPrnClaim is not null && long.TryParse(ukPrnClaim, out var ukPrn)
            ? ukPrn
            : default;
    }

    private async Task LoadProviderPostcodes(long ukPrn)
    {
        ProviderLocations = await _providerDataService.GetLocationPostcodes(ukPrn);

    }
}