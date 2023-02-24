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
    public async Task<IActionResult> UploadProviderContacts([FromForm] IFormFile file)
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
    [Route("towns")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadTowns([FromForm] IFormFile file)
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

            await using var ms = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension == ".csv")
            {
                await _townDataService.ImportTowns(ms);
            }
            else if (Path.GetExtension(file.FileName) == ".7z")
            {
                using var archive = new SevenZipArchive(ms);
                var entry = archive.Entries.FirstOrDefault(e => e.Name.EndsWith(".csv"));
                if (entry is null)
                {
                    _logger.LogWarning(
                        $"{nameof(DataImportController)} {nameof(UploadTowns)} zip archive has no csv file.");
                    return BadRequest("A 7-zip file containing a csv file is required.");
                }

                await using var entryStream = entry.Open();
                await _townDataService.ImportTowns(entryStream);
            }
            else if (Path.GetExtension(file.FileName) == ".zip")
            {
                using var zipArchive = new ZipArchive(ms, ZipArchiveMode.Read);
                var entry = zipArchive.Entries.FirstOrDefault(e => e.Name.EndsWith(".csv"));
                if (entry is null)
                {
                    _logger.LogWarning(
                        $"{nameof(DataImportController)} {nameof(UploadTowns)} zip archive has no csv file.");
                    return BadRequest("A zip file containing a csv file is required.");
                }

                await using var entryStream = entry.Open();
                await _townDataService.ImportTowns(entryStream);
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
