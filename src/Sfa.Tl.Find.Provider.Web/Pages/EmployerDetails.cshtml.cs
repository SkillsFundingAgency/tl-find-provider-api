using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Web.Pages;

//TODO: add security
[AllowAnonymous]
//[Authorize(nameof(PolicyNames.HasProviderAccount))]
public class EmployerDetailsModel : PageModel
{
    public Application.Models.EmployerInterestDetail? EmployerInterest { get; private set; }

    private readonly ILogger<EmployerDetailsModel> _logger;
    private readonly IEmployerInterestService _employerInterestService;

    public EmployerDetailsModel(
        IEmployerInterestService employerInterestService,
        ILogger<EmployerDetailsModel> logger)
    {
        _employerInterestService = employerInterestService ?? throw new ArgumentNullException(nameof(employerInterestService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task OnGet(int id)
    {
        EmployerInterest = await _employerInterestService.GetEmployerInterestDetail(id);
    }
}