﻿using Quartz;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;

namespace Sfa.Tl.Find.Provider.Api.Jobs;

public class ProviderNotificationEmailJob : IJob
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<ProviderNotificationEmailJob> _logger;

    public ProviderNotificationEmailJob(
        INotificationService notificationService,
        ILogger<ProviderNotificationEmailJob> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
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

            await _notificationService.SendProviderNotifications(frequency);

            _logger.LogInformation("{job} {frequency} job completed successfully.",
                nameof(ProviderNotificationEmailJob)
                , frequency);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(ProviderNotificationEmailJob)} job failed.");
        }
    }
}