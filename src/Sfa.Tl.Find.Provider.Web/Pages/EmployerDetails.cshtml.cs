using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Pages.EmployerInterest;

namespace Sfa.Tl.Find.Provider.Web.Pages;

//TODO: add security
[AllowAnonymous]
//[Authorize(nameof(PolicyNames.HasProviderAccount))]
public class EmployerDetailsModel : PageModel
{
    public Application.Models.EmployerInterest? EmployerInterest { get; private set; }

    private readonly ILogger<EmployerDetailsModel> _logger;
    private IEmployerInterestService _employerInterestService;

    public EmployerDetailsModel(
        IEmployerInterestService employerInterestService,
        ILogger<EmployerDetailsModel> logger)
    {
        _employerInterestService = employerInterestService ?? throw new ArgumentNullException(nameof(employerInterestService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnGet()
    {
    }

    //public async Task OnGet(int id)
    //{
    //    EmployerInterest = await _employerInterestService.GetEmployerInterest(id);
    //}
}