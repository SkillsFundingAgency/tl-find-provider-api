using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Quartz.Util;
using Sfa.Tl.Find.Provider.Api.Attributes;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Api.Controllers;

[ApiController]
[ApiVersion("3.0")]
[HmacAuthorization]
[Route("api/v{version:apiVersion}/[controller]")]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class ProvidersController : ControllerBase
{
    private readonly IProviderDataService _providerDataService;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILogger<ProvidersController> _logger;

    public ProvidersController(
        IProviderDataService providerDataService,
        IDateTimeService dateTimeService,
        ILogger<ProvidersController> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Search for providers.
    /// </summary>
    /// <param name="searchTerms">Postcode that the search should start from.</param>
    /// <param name="latitude">Latitude that the search should start from.</param>
    /// <param name="longitude">Longitude that the search should start from.</param>
    /// <param name="qualificationIds">Qualification ids to filter by. Optional, nulls or zeroes will be ignored.</param>
    /// <param name="routeIds">Route ids to filter by. Optional, nulls or zeroes will be ignored.</param>
    /// <param name="page">Page to be displayed (zero-based).</param>
    /// <param name="pageSize">Number of items to return on a page.</param>
    /// <returns>Json with providers.</returns>
    [HttpGet]
    [Route("", Name = "GetProviders")]
    [ProducesResponseType(typeof(ProviderSearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProviders(
        [FromQuery(Name = "searchTerm")]
        string searchTerms = null,
        [FromQuery(Name = "lat")]
        double? latitude = null,
        [FromQuery(Name = "lon")]
        double? longitude = null,
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
            if (!searchTerms.TryValidate(latitude, longitude, out var validationMessage))
            {
                return Ok(new ProviderSearchResponse
                {
                    Error = validationMessage
                });
            }

            var providersSearchResponse =
                searchTerms.IsNullOrWhiteSpace()
                    ? await _providerDataService.FindProviders(
                        latitude!.Value,
                        longitude!.Value,
                        routeIds,
                        qualificationIds,
                        page,
                        pageSize)
                    : await _providerDataService.FindProviders(
                        searchTerms,
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

    [HttpGet]
    [Route("all", Name = "GetAllProviderData")]
    [ProducesResponseType(typeof(ProviderDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllProviderData()
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug($"{nameof(ProvidersController)} {nameof(GetAllProviderData)} called.");
        }

        var providerDetailResponse = await _providerDataService.GetAllProviders();
        //throw new AbandonedMutexException();
        return Ok(providerDetailResponse);
    }

    [HttpGet]
    [Route("download", Name = "GetProviderDataAsCsv")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProviderDataAsCsv()
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug($"{nameof(ProvidersController)} {nameof(GetAllProviderData)} called.");
        }

        var bytes = await _providerDataService.GetCsv();

        var filePath = $"All T Level providers {_dateTimeService.Today:MMMM yyyy}.csv";

        //https://stackoverflow.com/questions/56279818/custom-http-headers-in-razor-pages
        HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
        return new FileContentResult(bytes, "text/csv")
        {
            FileDownloadName = filePath
        };
    }

    [HttpGet]
    [Route("download/size", Name = "GetProviderDataCsvFileSize")]
    [ProducesResponseType(typeof(ProviderDataDownloadInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProviderDataCsvFileInfo()
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug($"{nameof(ProvidersController)} {nameof(GetAllProviderData)} called.");
        }

        var bytes = await _providerDataService.GetCsv();

        //TODO: Cache

        var size = new ProviderDataDownloadInfoResponse
        {
            FileSize = bytes.Length
        };

        return Ok(size);
    }
}