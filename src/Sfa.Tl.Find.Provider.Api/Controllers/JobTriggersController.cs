using Microsoft.AspNetCore.Mvc;
using Quartz;
using Sfa.Tl.Find.Provider.Api.Attributes;
using Sfa.Tl.Find.Provider.Application.Models;
// ReSharper disable StringLiteralTypo

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
    [Route("importcoursedirectory")]
    public async Task TriggerCourseDirectoryImportJob() =>
        await TriggerJob(JobKeys.CourseDataImport);

    [HttpPost]
    [Route("employerinterestcleanup")]
    public async Task TriggerEmployerInterestCleanupJob() =>
        await TriggerJob(JobKeys.EmployerInterestCleanup);

    [HttpPost]
    [Route("providernotificationemailimmediate")]
    public async Task TriggerProviderNotificationEmailImmediateJob() =>
        await TriggerJob(JobKeys.ProviderNotificationEmailImmediate);

    [HttpPost]
    [Route("providernotificationemaildaily")]
    public async Task TriggerProviderNotificationEmailDailyJob() =>
        await TriggerJob(JobKeys.ProviderNotificationEmailDaily);

    [HttpPost]
    [Route("providernotificationemailweekly")]
    public async Task TriggerProviderNotificationEmailWeeklyJob() =>
        await TriggerJob(JobKeys.ProviderNotificationEmailWeekly);
    
    [HttpPost]
    [Route("importtowns")]
    public async Task TriggerImportTownDataJob() => 
        await TriggerJob(JobKeys.ImportTownData);

    private async Task TriggerJob(string jobKey)
    {
        _logger.LogInformation("Request to trigger {job} received",
            jobKey);

        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.TriggerJob(new JobKey(jobKey));
    }
}