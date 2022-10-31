using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;

namespace Sfa.Tl.Find.Provider.Web.Pages;

//TODO: add security
[AllowAnonymous]
//[Authorize(nameof(PolicyNames.EmployerInterestViewer))]
public class EmployerListModel : PageModel
{
    private readonly IEmployerInterestService _employerInterestService;
    private readonly IProviderDataService _providerDataService;
    private readonly EmployerInterestSettings _employerInterestSettings;
    private readonly ILogger<EmployerListModel> _logger;
    
    public IEnumerable<EmployerInterestSummary>? EmployerInterestList { get; private set; }

    public IEnumerable<LocationPostcode>? ProviderLocations { get; private set; }

    public int EmployerInterestRetentionWeeks =>
        (int)Math.Ceiling(_employerInterestSettings.RetentionDays / 7d);

    public int SearchRadius =>
        _employerInterestSettings.SearchRadius;

    public DateOnly? ServiceStartDate =>
        _employerInterestSettings?.ServiceStartDate is not null
            ? DateOnly.FromDateTime(_employerInterestSettings.ServiceStartDate.Value)
            : DateOnly.MinValue;

    public long? UkPrn { get; private set; }

    public EmployerListModel(
        IEmployerInterestService employerInterestService,
        IProviderDataService providerDataService,
        IOptions<EmployerInterestSettings> employerInterestOptions,
        ILogger<EmployerListModel> logger)
    {
        _employerInterestService = employerInterestService ?? throw new ArgumentNullException(nameof(employerInterestService));
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _employerInterestSettings = employerInterestOptions?.Value
                                    ?? throw new ArgumentNullException(nameof(employerInterestOptions));
    }

    public async Task OnGet()
    {
        UkPrn = GetUkPrn();
        await LoadProviderPostcodes(UkPrn);
    }

    private long? GetUkPrn()
    {
        var ukPrnClaim = HttpContext.User.GetClaim(CustomClaimTypes.UkPrn);
        return ukPrnClaim is not null && long.TryParse(ukPrnClaim, out var ukPrn)
            ? ukPrn
            : default;

    }

    private async Task LoadProviderPostcodes(long? ukPrn)
    {
        if (ukPrn is null) return;

        ProviderLocations = await _providerDataService
            .GetLocationPostcodes(ukPrn.Value);
    }
}