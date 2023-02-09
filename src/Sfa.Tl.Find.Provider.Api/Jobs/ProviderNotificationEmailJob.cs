using Quartz;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;

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
            var frequencyString = context
                .JobDetail
                .JobDataMap
                .GetString(JobDataKeys.NotificationFrequency);

            if (!Enum.TryParse<NotificationFrequency>(frequencyString, out var frequency))
            {
                throw new ArgumentException("A valid notification frequency was not found in the job data.");
            }

            await _providerDataService.SendProviderNotifications(frequency);

            _logger.LogInformation($"{nameof(ProviderNotificationEmailJob)} job completed successfully.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{nameof(ProviderNotificationEmailJob)} job failed.");
        }
    }
}