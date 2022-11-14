using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Api.Attributes;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Controllers;

[ApiController]
[ApiVersion("3.0")]
[HmacAuthorization]
[Route("api/v{version:apiVersion}/[controller]")]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class IndustriesController : ControllerBase
{
    private readonly IProviderDataService _providerDataService;
    private readonly ILogger<IndustriesController> _logger;

    public IndustriesController(
        IProviderDataService providerDataService,
        ILogger<IndustriesController> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Returns a list of all industries.
    /// </summary>
    /// <returns>Json with industries.</returns>
    [HttpGet]
    [Route("", Name = "GetIndustries")]
    [ProducesResponseType(typeof(IEnumerable<Application.Models.Route>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIndustries()
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug($"{nameof(IndustriesController)} {nameof(GetIndustries)} called.");
        }

        var industries = await _providerDataService.GetIndustries();
        return industries != null
            ? Ok(industries)
            : NotFound();
    }
}