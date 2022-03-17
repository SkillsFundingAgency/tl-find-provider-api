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
[HmacAuthorization]
[Route("api/v{version:apiVersion}/[controller]")]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class RoutesController : ControllerBase
{
    private readonly IProviderDataService _providerDataService;
    private readonly ILogger<RoutesController> _logger;

    public RoutesController(
        IProviderDataService providerDataService,
        ILogger<RoutesController> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
   
    /// <summary>
    /// Returns a list of all routes.
    /// </summary>
    /// <returns>Json with routes.</returns>
    [HttpGet]
    [Route("", Name = "GetRoutes")]
    [ProducesResponseType(typeof(IEnumerable<Route>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoutes()
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug($"{nameof(RoutesController)} {nameof(GetRoutes)} called.");
        }

        var routes = await _providerDataService.GetRoutes();
        return routes != null
            ? Ok(routes)
            : NotFound();
    }
}