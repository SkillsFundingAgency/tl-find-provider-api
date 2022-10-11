using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Extensions;
using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Web.Pages;

public class DashboardModel : PageModel
{
    private readonly ILogger<DashboardModel> _logger;

    public string? DisplayName { get; private set; }
    public string? Service { get; private set; }
    public string? UkPrn { get; private set; }

    public DashboardModel(
        ILogger<DashboardModel> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> OnGet()
    {
        var isAuthenticated = User.Identity.IsAuthenticated;

        var claims = User.Claims.ToList();
        foreach (var claim in claims)
        {
            Debug.WriteLine($"User claim {claim.Type} = {claim.Value}");
        }

        UkPrn = HttpContext.User.GetClaim(CustomClaimTypes.UkPrn);
        DisplayName = HttpContext.User.GetClaim(CustomClaimTypes.DisplayName);
        Service = HttpContext.User.GetClaim(CustomClaimTypes.Service);

        if (string.IsNullOrEmpty(UkPrn))
        {
            //TODO: This won't happen when login is working correctly
            return RedirectToPage("/ChooseOrganisation");
        }

        return Page();
    }
}