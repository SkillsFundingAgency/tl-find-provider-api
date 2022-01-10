using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Jobs;

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
        _logger.LogInformation($"{nameof(CourseDataImportJob)} job triggered. {context?.Trigger.JobKey.Name}");

        try
        {
            await _courseDirectoryService.ImportQualifications();
            await _courseDirectoryService.ImportProviders();

            _logger.LogInformation($"{nameof(CourseDataImportJob)} job completed successfully.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{nameof(CourseDataImportJob)} job failed.");
        }
    }
}