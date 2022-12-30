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
public class JobTriggersController : ControllerBase
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly ILogger<JobTriggersController> _logger;

    public JobTriggersController(
        ISchedulerFactory schedulerFactory,
        ILogger<JobTriggersController> logger)
    {
        _schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    // ReSharper disable once StringLiteralTypo
    [Route("importcoursedirectory")]
    public async Task TriggerCourseDirectoryImportJob()
    {
        const string jobKey = JobKeys.CourseDirectoryImport;
        _logger.LogInformation("Request to trigger {job} received",
            jobKey);

        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.TriggerJob(new JobKey(jobKey));
    }

    [HttpPost]
    // ReSharper disable once StringLiteralTypo
    [Route("employerinterestcleanup")]
    public async Task TriggerEmployerInterestCleanupJob()
    {
        const string jobKey = JobKeys.EmployerInterestCleanup;
        _logger.LogInformation("Request to trigger {job} received", 
            jobKey);

        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.TriggerJob(new JobKey(jobKey));
    }

    [HttpPost]
    // ReSharper disable once StringLiteralTypo
    [Route("startuptasks")]
    public async Task TriggerStartupTasksJob()
    {
        const string jobKey = JobKeys.StartupTasks;
        _logger.LogInformation("Request to trigger {job} received",
            jobKey);

        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.TriggerJob(new JobKey(jobKey));
    }
}