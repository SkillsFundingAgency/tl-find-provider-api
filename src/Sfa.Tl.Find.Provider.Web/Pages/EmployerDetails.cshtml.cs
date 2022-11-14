using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Web.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Pages;

[Authorize(nameof(PolicyNames.IsProviderOrAdministrator))]
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

    public async Task<IActionResult> OnGet(int id)
    {
        EmployerInterest = await _employerInterestService.GetEmployerInterestDetail(id);

        return EmployerInterest != null ? 
            Page() : 
            RedirectToPage("/Error/404");
    }
}