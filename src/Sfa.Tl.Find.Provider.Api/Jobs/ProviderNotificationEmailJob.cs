using Quartz;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Jobs;

public class ProviderNotificationEmailJob : IJob
{
    private readonly IProviderDataService _providerDataService;
    private readonly ILogger<ProviderNotificationEmailJob> _logger;

    public ProviderNotificationEmailJob(
        IProviderDataService providerDataService,
        ILogger<ProviderNotificationEmailJob> logger)
    {
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await _providerDataService.SendProviderNotifications();

            _logger.LogInformation($"{nameof(ProviderNotificationEmailJob)} job completed successfully.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{nameof(ProviderNotificationEmailJob)} job failed.");
        }
    }
}