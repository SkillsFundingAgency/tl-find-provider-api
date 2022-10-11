using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Extensions;

namespace Sfa.Tl.Find.Provider.Web.Pages;

[Authorize]
public class ChooseOrganisationModel : PageModel
{
    private readonly ILogger<ChooseOrganisationModel> _logger;

    public const string UkprnRegexString = @"^((?!(0))[0-9]{8})$";
    public static Regex UkprnRegex => new(@"^((?!(0))[0-9]{8})$");

    [BindProperty]
    [Required]
    [DisplayName("UKPRN")]
    //[Display(Name = "UKPRN")]
    [RegularExpression(UkprnRegexString, ErrorMessage = "UKPRN must be an 8 digit number")]
    public string? UkPrn { get; set; }

    public ChooseOrganisationModel(
        ILogger<ChooseOrganisationModel> logger)
    {
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

    public async Task<IActionResult> OnPost()
    {
        //Validate that it's a int of the correct length
        //fluent validation rule? see Matching

        if (!ModelState.IsValid)
        {
            //TODO: Validate this is a number of 8 digits
            return Page();
        }


        //save UkPrn to claims

        return RedirectToPage("/Dashboard");


    }
}