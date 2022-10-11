using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Web.Pages.EmployerInterest;

public class DetailModel : PageModel
{
    public Application.Models.EmployerInterest? EmployerInterest { get; private set; }

    private readonly ILogger<DetailModel> _logger;
    private IEmployerInterestService _employerInterestService;

    public DetailModel(
        IEmployerInterestService employerInterestService,
        ILogger<DetailModel> logger)
    {
        _employerInterestService = employerInterestService ?? throw new ArgumentNullException(nameof(employerInterestService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task OnGet(int id)
    {
        EmployerInterest = await _employerInterestService.GetEmployerInterest(id);
    }
}