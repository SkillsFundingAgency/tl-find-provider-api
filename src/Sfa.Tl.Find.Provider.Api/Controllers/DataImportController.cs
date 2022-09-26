using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Api.Attributes;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Controllers;

[ApiController]
[ApiVersion("3.0")]
[HmacAuthorization]
[Route("api/v{version:apiVersion}/[controller]")]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class DataImportController : ControllerBase
{
    private readonly IProviderDataService _providerDataService;
    private readonly ILogger<DataImportController> _logger;

    public DataImportController(
        IProviderDataService providerDataService,
        ILogger<DataImportController> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [HttpPost]
    [Route("provider/contacts")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadProviderContacts(IFormFile file)
    {
        try
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"{nameof(DataImportController)} {nameof(UploadProviderContacts)} called.");
            }

            if (file is null)
            {
                _logger.LogWarning($"{nameof(DataImportController)} {nameof(UploadProviderContacts)} has no file.");
                return BadRequest("File is required.");
            }
            
            await _providerDataService.ImportProviderContacts(
                file.OpenReadStream());

            return Accepted();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred. Returning error result.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    [HttpPost]
    [Route("provider/data-ping")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PingProviderData()
    {
        return Ok("hello");
    }

    [HttpGet]
    [HttpPost]
    [Route("provider/data2")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadProviderData2(IFormFile dataFile)
    {
        return Ok("yes-2");
    }

    [HttpGet]
    [HttpPost]
    [Route("provider/data3")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadProviderData3(
        [FromBody]IFormFile dataFile)
    {
        return Ok("yes-3");
    }

    [HttpGet]
    [HttpPost]
    [Route("provider/data")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadProviderData(IFormFile file)
    {
        try
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"{nameof(DataImportController)} {nameof(UploadProviderData)} called.");
            }

            if (file is null)
            {
                _logger.LogWarning($"{nameof(DataImportController)} {nameof(UploadProviderData)} has no file.");
                return BadRequest("File is required.");
            }

            const bool isAdditionalData = true;
            await _providerDataService.ImportProviderData(
                file.OpenReadStream(), isAdditionalData);

            return Accepted();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred. Returning error result.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
