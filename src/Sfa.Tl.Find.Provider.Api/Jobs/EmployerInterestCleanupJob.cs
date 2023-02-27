using Quartz;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Jobs;

[DisallowConcurrentExecution]
public class EmployerInterestCleanupJob : IJob
{
    private readonly IEmployerInterestService _employerInterestService;
    private readonly ILogger<EmployerInterestCleanupJob> _logger;

    public EmployerInterestCleanupJob(
        IEmployerInterestService employerInterestService,
        ILogger<EmployerInterestCleanupJob> logger)
    {
        _employerInterestService = employerInterestService ?? throw new ArgumentNullException(nameof(employerInterestService));
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

            await _employerInterestService.RemoveExpiredEmployerInterest();
            await _employerInterestService.NotifyExpiringEmployerInterest();

            _logger.LogInformation($"{nameof(EmployerInterestCleanupJob)} job completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(EmployerInterestCleanupJob)} job failed.");
        }
    }
}