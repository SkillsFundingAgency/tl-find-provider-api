using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Web.Pages.EmployerInterest;

public class IndexModel : PageModel
{
    public IEnumerable<Application.Models.EmployerInterest>? EmployerInterestList { get; private set; }

    private readonly ILogger<IndexModel> _logger;
    private IEmployerInterestService _employerInterestService;

    public IndexModel(
        IEmployerInterestService employerInterestService,
        ILogger<IndexModel> logger)
    {
        _employerInterestService = employerInterestService ?? throw new ArgumentNullException(nameof(employerInterestService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task OnGet()
    {
        EmployerInterestList = await _employerInterestService.FindEmployerInterest();
    }
}