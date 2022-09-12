using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sfa.Tl.Find.Provider.Web.Pages
{
    [AllowAnonymous]
    public class SignedOutModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
