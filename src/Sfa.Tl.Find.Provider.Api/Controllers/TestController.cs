using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Api.Attributes;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Api.Controllers;

[ApiController]
[ApiVersion("3.0")]
[HmacAuthorization]
[Route("api/v{version:apiVersion}/[controller]")]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class TestController : ControllerBase
{
    private readonly IEmailService _emailService;

    private readonly ILogger<TestController> _logger;

    public TestController(
        IEmailService emailService,
        ILogger<TestController> logger)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
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
    [Route("sendmail")]
    public async Task<IActionResult> GetSendEmail(
        [FromQuery(Name = "to")] string recipients)
    {
        var template = "TestWithoutPersonalisation";

        var tokens = new Dictionary<string, string>();

        var result = await _emailService.SendEmail(recipients, template, tokens);
        
        return Ok(result);
    }
}