using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sfa.Tl.Find.Provider.Web.Pages.Help;

[Authorize]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class IndexModel : PageModel
{
    public IList<string> UserClaims { get; set; }
        = new List<string>();

    public void OnGet()
    {
        foreach (var claim in User.Claims)
        {
            UserClaims.Add($"{claim.Type} = {claim.Value}");
        }
    }
}