using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Web.Controllers;

[ApiController]
//[ApiVersion("3.0")]
//[HmacAuthorization]
//[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class EmployersApiController : ControllerBase
{
    private readonly IEmployerInterestService _employerInterestService;
    private readonly ILogger<EmployersApiController> _logger;

    public EmployersApiController(
        IEmployerInterestService employerInterestService,
        ILogger<EmployersApiController> logger)
    {
        _employerInterestService = employerInterestService ?? throw new ArgumentNullException(nameof(employerInterestService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates an Employer Interest record.
    /// </summary>
    /// <returns>A status result indicating whether the action succeeded, containing the unique id of the created object.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    // ReSharper disable once StringLiteralTypo
    [Route("createinterest")]
    public async Task<IActionResult> CreateInterest(EmployerInterest employerInterest)
    {
        _logger.LogInformation($"{nameof(EmployersApiController)} {nameof(CreateInterest)} called.");

        //TODO: Validate the model

        Debug.WriteLine("CreateInterest called with:");
        Debug.WriteLine($"   Organisation name:\t{employerInterest.OrganisationName}");
        Debug.WriteLine($"   Contact name:\t{employerInterest.ContactName}");
        Debug.WriteLine($"   Postcode:\t{employerInterest.Postcode}");
        Debug.WriteLine($"   Email:\t{employerInterest.Email}");
        Debug.WriteLine($"   Telephone:\t{employerInterest.Telephone}");
        Debug.WriteLine($"   Website:\t{employerInterest.Website}");
        Debug.WriteLine($"   Contact preference:\t{employerInterest.ContactPreferenceType}");
        Debug.WriteLine($"   Industry:\t{employerInterest.IndustryId}");
        Debug.WriteLine($"   Other industry:\t{employerInterest.OtherIndustry}");
        Debug.WriteLine($"   Additional info:\t{employerInterest.AdditionalInformation}");
        if (employerInterest.SkillAreaIds != null && employerInterest.SkillAreaIds.Any())
        {
            Debug.WriteLine($"   Skill area:\t{employerInterest.AdditionalInformation}");
            foreach (var skillArea in employerInterest.SkillAreaIds)
            {
                Debug.WriteLine($"   Skill area:\t{skillArea}");
            }
        }

        var uniqueId = await _employerInterestService.CreateEmployerInterest(employerInterest);

        return Ok(new
        {
            id = uniqueId
        });
    }
}