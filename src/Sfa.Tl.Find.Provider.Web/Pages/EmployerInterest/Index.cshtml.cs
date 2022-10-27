using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Extensions;

namespace Sfa.Tl.Find.Provider.Web.Pages.EmployerInterest;

[Authorize(nameof(PolicyNames.EmployerInterestViewer))]
public class IndexModel : PageModel
{
    public IEnumerable<EmployerInterestSummary>? EmployerInterestList { get; private set; }

    public int? TotalEmployerInterestItems { get; private set; }

    public IEnumerable<LocationPostcode>? ProviderLocations { get; private set; }

    public bool IsProvider => UkPrn.HasValue;

    public long? UkPrn { get; private set; }

    [BindProperty]
    public string? Postcode { get; set; }

    public int EmployerInterestRetentionDays =>
        _employerInterestSettings.RetentionDays;

    public int EmployerInterestRetentionWeeks =>
        (int)Math.Ceiling(_employerInterestSettings.RetentionDays / 7d);

    public int SearchRadius =>
        _employerInterestSettings.SearchRadius;
    
    public DateOnly? ServiceStartDate =>
        _employerInterestSettings?.ServiceStartDate is not null
            ? DateOnly.FromDateTime(_employerInterestSettings.ServiceStartDate.Value)
            : DateOnly.MinValue;

    private IEmployerInterestService _employerInterestService;
    private IProviderDataService _providerDataService;
    private readonly EmployerInterestSettings _employerInterestSettings;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        IEmployerInterestService employerInterestService,
        IProviderDataService providerDataService,
        IOptions<EmployerInterestSettings> employerInterestOptions,
        ILogger<IndexModel> logger)
    {
        _employerInterestService =
            employerInterestService ?? throw new ArgumentNullException(nameof(employerInterestService));
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _employerInterestSettings = employerInterestOptions?.Value
                                    ?? throw new ArgumentNullException(nameof(employerInterestOptions));
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
            (EmployerInterestList, TotalEmployerInterestItems) = await _employerInterestService.FindEmployerInterest(Postcode);
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