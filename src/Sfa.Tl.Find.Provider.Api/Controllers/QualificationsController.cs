using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sfa.Tl.Find.Provider.Api.Attributes;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Controllers;

[ApiController]
[ApiVersion("2.0")]
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
}