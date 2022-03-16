﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Controllers;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class LocationsController : ControllerBase
{
    private readonly ITownDataService _townDataService;
    // ReSharper disable once NotAccessedField.Local
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(
        ITownDataService townDataService,
        ILogger<LocationsController> logger)
    {
        _townDataService = townDataService ?? throw new ArgumentNullException(nameof(townDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// Search for locations by partial name.
    /// </summary>
    /// <param name="searchString">Search string.</param>
    /// <returns>A list of results.</returns>
    [HttpGet]
    [Route("search", Name = "SearchLocations")]
    [ProducesResponseType(typeof(IEnumerable<Town>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(string searchString)
    {
        var towns = await _townDataService.Search(searchString);
        return Ok(towns);
    }
}