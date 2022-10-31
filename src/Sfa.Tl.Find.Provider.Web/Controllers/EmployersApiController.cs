using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;

namespace Sfa.Tl.Find.Provider.Web.Controllers;

[ApiController]
//[ApiVersion("3.0")]
//[HmacAuthorization]
//[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/employers")]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class EmployersApiController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly IEmployerInterestService _employerInterestService;
    private readonly ILogger<EmployersApiController> _logger;

    public EmployersApiController(
        IMemoryCache cache,
        IEmployerInterestService employerInterestService,
        ILogger<EmployersApiController> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
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

        var uniqueId = await _employerInterestService.CreateEmployerInterest(employerInterest);

        return Ok(new
        {
            id = uniqueId
        });
    }

    [HttpDelete]
    [AllowAnonymous]
    // ReSharper disable once StringLiteralTypo
    [Route("deletecacheduser")]
    public async Task<IActionResult> DeleteCachedUser()
    {
        _logger.LogInformation($"{nameof(EmployersApiController)} {nameof(DeleteCachedUser)} called.");

        _cache.Remove(User.GetUserSessionCacheKey());

        return NoContent();
    }
}