using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;

public class NotificationServiceBuilder
{
    public INotificationService Build(
        IDateTimeProvider dateTimeProvider = null,
        IGuidProvider guidProvider = null,
        IEmailService emailService = null,
        INotificationRepository notificationRepository = null,
        ProviderSettings providerSettings = null,
        SearchSettings searchSettings = null,
        ILogger<NotificationService> logger = null)
    {
        dateTimeProvider ??= Substitute.For<IDateTimeProvider>();
        guidProvider ??= Substitute.For<IGuidProvider>();
        emailService ??= Substitute.For<IEmailService>();
        notificationRepository ??= Substitute.For<INotificationRepository>();
        logger ??= Substitute.For<ILogger<NotificationService>>();

        providerSettings ??= new SettingsBuilder().BuildProviderSettings();
        searchSettings ??= new SettingsBuilder().BuildSearchSettings();

        return new NotificationService(
            dateTimeProvider,
            guidProvider,
            emailService,
            notificationRepository,
            providerSettings.ToOptions(),
            searchSettings.ToOptions(),
            logger);
    }
}