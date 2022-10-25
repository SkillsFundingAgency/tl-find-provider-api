using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Extensions;

namespace Sfa.Tl.Find.Provider.Web.Pages;

//TODO: add security
//[AllowAnonymous]
//[Authorize(nameof(PolicyNames.EmployerInterestViewer))]
public class EmployerListModel : PageModel
{
    private IEmployerInterestService _employerInterestService;
    private IProviderDataService _providerDataService;
    private readonly ILogger<EmployerListModel> _logger;

    public EmployerListModel(
        IEmployerInterestService employerInterestService,
        IProviderDataService providerDataService,
        ILogger<EmployerListModel> logger)
    {
        _employerInterestService = employerInterestService ?? throw new ArgumentNullException(nameof(employerInterestService));
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task OnGet()
    {
        var ukPrn = HttpContext.User.GetClaim(CustomClaimTypes.UkPrn);
    }
}