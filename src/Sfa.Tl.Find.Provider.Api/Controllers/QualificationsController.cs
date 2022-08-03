using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Api.Attributes;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Api.Controllers;

[ApiController]
[ApiVersion("3.0")]
[HmacAuthorization]
[Route("api/v{version:apiVersion}/[controller]")]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class QualificationsController : ControllerBase
{
    private readonly IProviderDataService _providerDataService;
    private readonly ILogger<QualificationsController> _logger;

    public QualificationsController(
        IProviderDataService providerDataService,
        ILogger<QualificationsController> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
   
    /// <summary>
    /// Returns a list of all qualifications.
    /// </summary>
    /// <returns>Json with qualifications.</returns>
    [HttpGet]
    [Route("", Name = "GetQualifications")]
    [ProducesResponseType(typeof(IEnumerable<Qualification>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQualifications()
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug($"{nameof(QualificationsController)} {nameof(GetQualifications)} called.");
        }

        var qualifications = await _providerDataService.GetQualifications();
        return qualifications != null
            ? Ok(qualifications)
            : NotFound();
    }

    [HttpPost]
    public IActionResult PostQualification(Qualification qualification)
    {
        _logger.LogInformation($"{nameof(QualificationsController)} {nameof(PostQualification)} called " +
                         " with {id} {name}", qualification.Id, qualification.Name);

        return Ok();
    }
}