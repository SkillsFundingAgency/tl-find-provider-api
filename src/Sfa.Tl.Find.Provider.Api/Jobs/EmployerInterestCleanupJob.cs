using Quartz;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Jobs;

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
            await _employerInterestService.RemoveExpiredEmployerInterest();

            _logger.LogInformation($"{nameof(EmployerInterestCleanupJob)} job completed successfully.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{nameof(EmployerInterestCleanupJob)} job failed.");
        }
    }
}