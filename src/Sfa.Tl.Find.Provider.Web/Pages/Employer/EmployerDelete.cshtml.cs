using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Pages.Employer;

[Authorize(Roles = CustomRoles.Administrator)]
public class EmployerDeleteModel : PageModel
{
    public EmployerInterestDetail? EmployerInterest { get; private set; }

    private readonly IEmployerInterestService _employerInterestService;
    private readonly ILogger<EmployerDetailsModel> _logger;

    public EmployerDeleteModel(
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

    public async Task<IActionResult> OnPost(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        EmployerInterest = await _employerInterestService.GetEmployerInterestDetail(id.Value);

        TempData["DeletedOrganisationName"] = EmployerInterest.OrganisationName;

        await _employerInterestService.DeleteEmployerInterest(id.Value);

        return RedirectToPage("/Employer/EmployerList");
    }
}
