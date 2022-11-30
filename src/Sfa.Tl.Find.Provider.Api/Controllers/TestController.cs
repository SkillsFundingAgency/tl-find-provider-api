using Microsoft.AspNetCore.Mvc;
using Quartz;
using Sfa.Tl.Find.Provider.Api.Attributes;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Api.Controllers;

[ApiController]
[ApiVersion("3.0")]
[HmacAuthorization]
[Route("api/v{version:apiVersion}/[controller]")]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class TestController : ControllerBase
{
    // ReSharper disable once NotAccessedField.Local
    private readonly ILogger<TestController> _logger;
    private readonly ISchedulerFactory _schedulerFactory;

    public TestController(
        ILogger<TestController> logger,
        ISchedulerFactory schedulerFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
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
    
    [HttpGet, HttpPost]
    // ReSharper disable once StringLiteralTypo
    [Route("triggeremployerinterestcleanupjob")]
    public async Task TriggerEmployerInterestCleanupJob(
        [FromQuery(Name = "to")] string recipients)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.TriggerJob(new JobKey(Constants.EmployerInterestCleanupJobKeyName));
    }
}