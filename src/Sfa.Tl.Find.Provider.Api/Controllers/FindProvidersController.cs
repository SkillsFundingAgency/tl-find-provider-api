﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sfa.Tl.Find.Provider.Api.Attributes;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
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
        int pageSize = Constants.DefaultPageSize)
    {
        try
        {
            if (!TryValidate(postcode, out var validationMessage))
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

    private static bool TryValidate(string postcode, out string errorMessage)
    {
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(postcode))
        {
            errorMessage = "The postcode field is required.";
        }
        else
        {
            errorMessage = postcode.Length switch
            {
                < 2 => "The postcode field must be at least 2 characters.",
                > 8 => "The postcode field must be no more than 8 characters.",
                _ => errorMessage
            };

            var regex = new Regex(@"^[a-zA-Z][0-9a-zA-Z\s]*$");
            if (!regex.IsMatch(postcode))
                errorMessage = "The postcode field must start with a letter and contain only letters, numbers, and an optional space.";
        }

        return errorMessage is null;
    }
}