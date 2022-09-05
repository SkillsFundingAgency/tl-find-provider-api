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

    [HttpPost]
    [Route("provider/contacts")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
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

            using var reader = new StreamReader(file.OpenReadStream());
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
    [Route("provider/contacts")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUploadProviderContacts(
        [FromForm] IFormFile file)
    {
        try
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"{nameof(DataImportController)} {nameof(GetUploadProviderContacts)} called.");
            }

            if (file is null)
            {
                _logger.LogWarning($"{nameof(DataImportController)} {nameof(UploadProviderContacts)} has no file.");
                return BadRequest("File is required.");
            }

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var bytes = ms.ToArray();

            await _providerDataService.ImportProviderContacts(
                bytes);

            return Accepted();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred. Returning error result.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    //TODO: Document this change - extra optional bit on amx header
    //TODO: Retest all from postman and Zendesk
    /*
    const skipBodyEncoding = true; //Set this to true if body encoking should be skipped

    const appId = "TODO";
    const apiKey = "TODO";
    const uri = pm.request.url.toString();
    const ts = Math.round((new Date()).getTime() / 1000);
    const uuid = //createUniqueId();
        'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g,
            function(c) {
                var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
    const nonce = CryptoJS.enc.Hex.parse(uuid);

    var requestContentBase64String = "";

    if(!skipBodyEncoding) {
        var bodyString = pm.request.body.toString();//interpolate(pm.request.body.toString());

        if (bodyString) {
            console.log("encoding body");
            var md5 = CryptoJS.MD5(bodyString);
            requestContentBase64String = CryptoJS.enc.Base64.stringify(md5);
        }
    }

    const dataToHash = `${appId}${pm.request.method}${uri.toLowerCase()}${ts}${nonce}${requestContentBase64String}`;
    const hash = CryptoJS.HmacSHA256(dataToHash, apiKey);
    const hashInBase64 = CryptoJS.enc.Base64.stringify(hash);

    let amxHeader = `amx ${appId}:${hashInBase64}:${nonce}:${ts}`;
    if(skipBodyEncoding) amxHeader += ":true";
    postman.setEnvironmentVariable("hmac",  amxHeader);
    */
}
