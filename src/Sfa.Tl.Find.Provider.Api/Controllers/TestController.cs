using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Api.Attributes;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Api.Controllers;

[ApiController]
[ApiVersion("3.0")]
[HmacAuthorization]
[Route("api/v{version:apiVersion}/[controller]")]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class TestController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly EmailSettings _emailSettings;

    private readonly ILogger<TestController> _logger;

    public TestController(
        IEmailService emailService,
        IOptions<EmailSettings> emailOptions,
        ILogger<TestController> logger)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _emailSettings = emailOptions?.Value
                         ?? throw new ArgumentNullException(nameof(emailOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    public IActionResult PostQualification(Qualification qualification)
    {
        _logger.LogInformation($"{nameof(TestController)} {nameof(PostQualification)} called " +
                         " with {id} {name}", qualification.Id, qualification.Name);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetSendTestEmail(
        //[FromQuery(Name = "toAddress")] string toAddress
        )
    {
        //Can use EmailSettings.SupportEmailAddress
        var template = "TestWithoutPersonalisation";
        var toAddress = _emailSettings.SupportEmailAddress;
        var tokens = new Dictionary<string, string>();

        var result = await _emailService.SendEmail(toAddress, template, tokens);
        return Ok(result);
    }
}