using Microsoft.AspNetCore.Mvc;
using Quartz;
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
    private readonly ISchedulerFactory _schedulerFactory;

    public TestController(
        IEmailService emailService,
        ILogger<TestController> logger,
        ISchedulerFactory schedulerFactory)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
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
        const string template = "TestWithoutPersonalisation";

        var tokens = new Dictionary<string, string>();

        var result = await _emailService.SendEmail(recipients, template, tokens);

        return Ok(result);
    }

    [HttpGet]
    // ReSharper disable once StringLiteralTypo
    [Route("sendemployermail")]
    public async Task<IActionResult> GetSendEmployerEmail(
        [FromQuery(Name = "to")] string recipients)
    {
        var template = "EmployerInterest";

        var tokens = new Dictionary<string, string>()
        {
            {"employer_name", "Test Employer"},
            {"employer_phone_number", "+44 012 345 6789"},
            {"employer_email_address", "@test.co.uk"},
            {
                "providers_list",
                "* Provider in location 1\r\n" +
                "* Another provider in location 2"
            }
        };

        var result = await _emailService.SendEmail(recipients, template, tokens);

        return Ok(result);
    }

    [HttpGet, HttpPost]
    // ReSharper disable once StringLiteralTypo
    [Route("triggerimportjob")]
    public async Task TriggerCourseDirectoryImportJob(
        [FromQuery(Name = "to")] string recipients)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.TriggerJob(new JobKey(Constants.CourseDirectoryImportJobKeyName));
    }

    [HttpGet, HttpPost]
    // ReSharper disable once StringLiteralTypo
    [Route("triggerstartuptasksjob")]
    public async Task TriggerStartupTasksJob(
        [FromQuery(Name = "to")] string recipients)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.TriggerJob(new JobKey(Constants.StartupTasksJobKeyName));
    }
}