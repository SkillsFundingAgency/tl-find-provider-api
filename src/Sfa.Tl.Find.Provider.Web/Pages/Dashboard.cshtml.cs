using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Extensions;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly ILogger<DashboardModel> _logger;

    public string? OrganisationName { get; private set; }
    public string? UkPrn { get; private set; }
    public string? Urn { get; private set; }

    public DashboardModel(
        ILogger<DashboardModel> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> OnGet()
    {
        foreach (var claim in User.Claims)
        {
            Debug.WriteLine($"User claim {claim.Type} = {claim.Value}");
        }

        UkPrn = HttpContext.User.GetClaim(CustomClaimTypes.UkPrn);
        Urn = HttpContext.User.GetClaim(CustomClaimTypes.Urn);
        OrganisationName = HttpContext.User.GetClaim(CustomClaimTypes.OrganisationName);

        return Page();
    }
}