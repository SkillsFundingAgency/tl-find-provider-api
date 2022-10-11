using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Extensions;

namespace Sfa.Tl.Find.Provider.Web.Pages;

[Authorize]
public class ChooseOrganisationModel : PageModel
{
    private readonly ILogger<ChooseOrganisationModel> _logger;
    private readonly UkPrnClaimsTransformation _ukPrnClaimsTransformation;

    private const string UkprnRegexString = @"^((?!(0))[0-9]{8})$";

    [BindProperty]
    [Required]
    [DisplayName("UKPRN")]
    [RegularExpression(UkprnRegexString, ErrorMessage = "UKPRN must be an 8 digit number")]
    public string? UkPrn { get; set; }

    public ChooseOrganisationModel(
        IClaimsTransformation claimsTransformation,
        ILogger<ChooseOrganisationModel> logger)
    {
        _ukPrnClaimsTransformation = (UkPrnClaimsTransformation)claimsTransformation ?? throw new ArgumentNullException(nameof(claimsTransformation));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnGet()
    {
        try
        {
            UkPrn = HttpContext.User.GetClaim(CustomClaimTypes.UkPrn);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _logger.LogError(e, "Exception when loading UKPRN claim");
            throw;
        }
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _ukPrnClaimsTransformation.UkPrn = UkPrn;
        return RedirectToPage("/Dashboard");
    }
}