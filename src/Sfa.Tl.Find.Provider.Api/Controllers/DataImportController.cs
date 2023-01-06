using Aspose.Zip.SevenZip;
using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Api.Attributes;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using System.IO.Compression;

namespace Sfa.Tl.Find.Provider.Api.Controllers;

[ApiController]
[ApiVersion("3.0")]
[HmacAuthorization]
[Route("api/v{version:apiVersion}/[controller]")]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class DataImportController : ControllerBase
{
    private readonly IProviderDataService _providerDataService;
    private readonly ITownDataService _townDataService;
    private readonly ILogger<DataImportController> _logger;

    public DataImportController(
        IProviderDataService providerDataService,
        ITownDataService townDataService,
        ILogger<DataImportController> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _townDataService = townDataService ?? throw new ArgumentNullException(nameof(townDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

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

    [HttpPost]
    [Route("provider/data")]
    //[RequestSizeLimit(500 * 1024 * 1024)] //unit is bytes => 500Mb
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,//500 * 1024 * 1024,
                       BufferBody = true,
                       BufferBodyLengthLimit = int.MaxValue,
                       ValueLengthLimit = int.MaxValue,
                       MultipartBoundaryLengthLimit = int.MaxValue,
                       MultipartHeadersCountLimit = int.MaxValue,
                       MultipartHeadersLengthLimit = int.MaxValue,
                       ValueCountLimit = int.MaxValue)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadProviderData([FromForm] IFormFile file)
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

    [HttpPost]
    [Route("provider/data/zip")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadProviderDataZipped([FromForm] IFormFile file)
    {
        const bool isAdditionalData = true;
        try
        {
            _logger.LogInformation("{className} {methodName} called with file {fileName}.", 
                    nameof(DataImportController),
                    nameof(UploadProviderDataZipped), 
                    file?.FileName);

            if (file is null)
            {
                _logger.LogWarning($"{nameof(DataImportController)} {nameof(UploadProviderData)} has no file.");
                return BadRequest("File is required.");
            }

            await using var ms = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);

            if (Path.GetExtension(file.FileName) == ".7z")
            {
                using var archive = new SevenZipArchive(ms);
                var entry = archive.Entries.FirstOrDefault(e => e.Name.EndsWith(".json"));
                if (entry is null)
                {
                    _logger.LogWarning(
                        $"{nameof(DataImportController)} {nameof(UploadProviderData)} zip archive has no json file.");
                    return BadRequest("A zip file containing a json file is required.");
                }

                await using var entryStream = entry.Open();
                await _providerDataService.ImportProviderData(
                    entryStream, isAdditionalData);
            }
            else
            {
                using var zipArchive = new ZipArchive(ms, ZipArchiveMode.Read);
                var entry = zipArchive.Entries.FirstOrDefault(e => e.Name.EndsWith(".json"));
                if (entry is null)
                {
                    _logger.LogWarning(
                        $"{nameof(DataImportController)} {nameof(UploadProviderData)} zip archive has no json file.");
                    return BadRequest("A zip file containing a json file is required.");
                }

                await using var entryStream = entry.Open();
                await _providerDataService.ImportProviderData(
                    entryStream, isAdditionalData);
            }

            return Accepted();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred. Returning error result.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    [Route("towns")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadTowns(IFormFile file)
    {
        _logger.LogInformation($"{nameof(DataImportController)} {nameof(UploadTowns)} called.");
        try
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"{nameof(DataImportController)} {nameof(UploadTowns)} called.");
            }

            if (file is null)
            {
                _logger.LogWarning($"{nameof(DataImportController)} {nameof(UploadTowns)} has no file.");
                return BadRequest("File is required.");
            }

            await _townDataService.ImportTowns(
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
    [Route("towns3")]
    public async Task<IActionResult> UploadTowns3()
    {
            _logger.LogInformation($"{nameof(DataImportController)} {nameof(UploadTowns3)} called.");
            return Ok();
    }

    [HttpPost]
    [Route("towns2")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadTowns2(IFormFile file)
    {
        try
        {
            _logger.LogInformation($"{nameof(DataImportController)} {nameof(UploadTowns2)} called.");

            if (file is null)
            {
                _logger.LogWarning($"{nameof(DataImportController)} {nameof(UploadTowns)} has no file.");
                return BadRequest("File is required.");
            }

            return Accepted();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred. Returning error result.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
