﻿using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Api.Attributes;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Controllers;

[ApiController]
[ApiVersion("3.0")]
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
    [ProducesResponseType(typeof(IEnumerable<Application.Models.Route>), StatusCodes.Status200OK)]
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