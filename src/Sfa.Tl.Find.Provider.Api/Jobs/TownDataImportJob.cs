using Quartz;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Jobs;

[DisallowConcurrentExecution]
public class TownDataImportJob : IJob
{
    private readonly ITownDataService _townDataService;
    private readonly ILogger<TownDataImportJob> _logger;

    public TownDataImportJob(
        ITownDataService townDataService,
        ILogger<TownDataImportJob> logger)
    {
        _townDataService = townDataService ?? throw new ArgumentNullException(nameof(townDataService));
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
                context?.Trigger.JobKey.Name);

            await _townDataService.ImportTowns();

            _logger.LogInformation($"{nameof(TownDataImportJob)} job completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(TownDataImportJob)} job failed.");
        }
    }
}