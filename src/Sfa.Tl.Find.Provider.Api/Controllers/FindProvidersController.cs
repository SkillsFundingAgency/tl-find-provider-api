using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sfa.Tl.Find.Provider.Api.Attributes;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[HmacAuthorization]
[Route("api/v{version:apiVersion}/[controller]")]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class FindProvidersController : ControllerBase
{
    private readonly IProviderDataService _providerDataService;
    private readonly ILogger<FindProvidersController> _logger;

    public FindProvidersController(
        IProviderDataService providerDataService,
        ILogger<FindProvidersController> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Search for providers.
    /// </summary>
    /// <param name="postcode">Postcode that the search should start from.</param>
    /// <param name="searchTerm">Can be used for postcode - allowed in case a v1 uri is used with v2 parameters.</param>
    /// <param name="qualificationIds">Qualification ids to filter by. Optional, nulls or zeroes will be ignored.</param>
    /// <param name="routeIds">Route ids to filter by. Optional, nulls or zeroes will be ignored.</param>
    /// <param name="page">Page to be displayed (zero-based).</param>
    /// <param name="pageSize">Number of items to return on a page.</param>
    /// <returns>Json with providers.</returns>
    [HttpGet]
    [Route("providers", Name = "GetProvidersV1")]
    [ProducesResponseType(typeof(ProviderSearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProviders(
        [FromQuery]
        string postcode,
        [FromQuery(Name = "routeId")]
        IList<int> routeIds = null,
        [FromQuery(Name = "qualificationId")]
        IList<int> qualificationIds = null,
        [FromQuery,
         Range(0, int.MaxValue, ErrorMessage = "The page field must be zero or greater.")]
        int page = 0,
        [FromQuery,
         Range(1, int.MaxValue, ErrorMessage = "The pageSize field must be at least one.")]
        int pageSize = Constants.DefaultPageSize,
        [FromQuery]
        string searchTerm = null)
    {
        try
        {
            if (postcode == null && searchTerm != null)
            {
                postcode = searchTerm;
            }

            if (!postcode.TryValidate(out var validationMessage))
            {
                return Ok(new ProviderSearchResponse
                {
                    Error = validationMessage
                });
            }

            var providersSearchResponse = await _providerDataService.FindProviders(
                postcode,
                routeIds,
                qualificationIds,
                page,
                pageSize);

            return Ok(providersSearchResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred. Returning error result.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    /// <summary>
    /// Returns a list of all qualifications.
    /// </summary>
    /// <returns>Json with qualifications.</returns>
    [HttpGet]
    [Route("qualifications", Name = "GetQualificationsV1")]
    [ProducesResponseType(typeof(IEnumerable<Qualification>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQualifications()
    {
        var qualifications = await _providerDataService.GetQualifications();
        return qualifications != null
            ? Ok(qualifications)
            : NotFound();
    }

    /// <summary>
    /// Returns a list of all routes.
    /// </summary>
    /// <returns>Json with routes.</returns>
    [HttpGet]
    [Route("routes", Name = "GetRoutesV1")]
    [ProducesResponseType(typeof(IEnumerable<Route>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoutes()
    {
        var routes = await _providerDataService.GetRoutes();
        return routes != null
            ? Ok(routes)
            : NotFound();
    }
}