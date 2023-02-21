using Quartz;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Jobs;

[DisallowConcurrentExecution]
public class CourseDataImportJob : IJob
{
    private readonly ICourseDirectoryService _courseDirectoryService;
    private readonly ILogger<CourseDataImportJob> _logger;

    public CourseDataImportJob(
        ICourseDirectoryService courseDirectoryService,
        ILogger<CourseDataImportJob> logger)
    {
        _courseDirectoryService = courseDirectoryService ?? throw new ArgumentNullException(nameof(courseDirectoryService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            if ((await context.Scheduler.GetCurrentlyExecutingJobs())
                .Any(x =>
                    x.FireInstanceId != context.FireInstanceId
                    && x.JobDetail.Key.Equals(context.JobDetail.Key)))
            {
                _logger.LogInformation("Duplicate job detected for {jobKey} - exiting immediately.",
                    context.JobDetail.Key.Name);
                return;
            }

            _logger.LogInformation("{jobKey} job triggered.",
                context.Trigger.JobKey.Name);

            //await _courseDirectoryService.ImportQualifications();
            await _courseDirectoryService.ImportProviders();

            _logger.LogInformation($"{nameof(CourseDataImportJob)} job completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(CourseDataImportJob)} job failed.");
        }
    }
}