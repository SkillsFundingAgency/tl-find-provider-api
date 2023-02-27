using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Api.Jobs;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Jobs;
public class ProviderNotificationEmailJobBuilder
{
    public ProviderNotificationEmailJob Build(
        INotificationService notificationService = null,
        ILogger<ProviderNotificationEmailJob> logger = null)
    {
        notificationService ??= Substitute.For<INotificationService>();
        logger ??= Substitute.For<ILogger<ProviderNotificationEmailJob>>();

        return new ProviderNotificationEmailJob(
            notificationService,
            logger);
    }
}
