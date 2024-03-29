using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Pages.Employer;

[Authorize(Roles = CustomRoles.Administrator)]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class RemoveEmployerModel : PageModel
{
    public EmployerInterestDetail? EmployerInterest { get; private set; }

    private readonly IEmployerInterestService _employerInterestService;
    private readonly ILogger<RemoveEmployerModel> _logger;

    public RemoveEmployerModel(
        IEmployerInterestService employerInterestService,
        ILogger<RemoveEmployerModel> logger)
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
        
        await _employerInterestService.DeleteEmployerInterest(id.Value);

        TempData[nameof(EmployerListModel.DeletedOrganisationName)] = EmployerInterest?.OrganisationName;

        return RedirectToPage("/Employer/EmployerList");
    }
}
