using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sfa.Tl.Find.Provider.Web.Pages.Help;

[Authorize]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class InfoModel : PageModel
{
    public Dictionary<string, string> UserClaims { get; set; } = new();
    public List<string> UserRoles { get; set; } = new();

    public void OnGet()
    {
        var identity = User.Identity as ClaimsIdentity;

        foreach (var claim in User.Claims)
        {
            if (claim.Type == identity?.RoleClaimType)
            {
                UserRoles.Add($"{claim.Value}");
            }
            else
            {
                var claimType = claim.Type.Contains('/') 
                    ? claim.Type.Remove(0, claim.Type.LastIndexOf('/') + 1) 
                    : claim.Type;
                UserClaims.Add(claimType, claim.Value);
            }
        }
    }
}