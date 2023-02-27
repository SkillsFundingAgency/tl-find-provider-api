using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Api.Attributes;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Api.Controllers;

[ApiController]
[ApiVersion("3.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class LocationsController : ControllerBase
{
    private readonly ITownDataService _townDataService;
    private readonly IPostcodeLookupService _postcodeLookupService;
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(
        ITownDataService townDataService,
        IPostcodeLookupService postcodeLookupService,
        ILogger<LocationsController> logger)
    {
        _townDataService = townDataService ?? throw new ArgumentNullException(nameof(townDataService));
        _postcodeLookupService = postcodeLookupService ?? throw new ArgumentNullException(nameof(postcodeLookupService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Search for locations by partial name.
    /// </summary>
    /// <param name="searchTerm">Search string.</param>
    /// <returns>A list of results.</returns>
    [HttpGet]
    [Route("", Name = "SearchLocations")]
    [ProducesResponseType(typeof(IEnumerable<Town>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(string searchTerm)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug($"{nameof(LocationsController)} {nameof(Search)} called.");
        }

        var towns = await _townDataService.Search(searchTerm);
        return Ok(towns);
    }

    [HmacAuthorization]
    [HttpGet]
    [Route("validate", Name = "ValidatePostcode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ValidatePostcode(string postcode)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug($"{nameof(LocationsController)} {nameof(ValidatePostcode)} called.");
        }

        var result = false;

        try
        {
            result = !string.IsNullOrEmpty(postcode)
                     && await _postcodeLookupService.IsValid(postcode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ValidatePostcode");
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("ValidatePostcode result is {result}.",
                result);
        }

        return result
            ? Ok()
            : NotFound();
    }
}