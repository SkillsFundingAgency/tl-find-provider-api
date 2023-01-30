using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Api.Jobs;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Jobs;
public class ProviderNotificationEmailJobBuilder
{
    public ProviderNotificationEmailJob Build(
        IProviderDataService providerDataService = null,
        ILogger<ProviderNotificationEmailJob> logger = null)
    {
        providerDataService ??= Substitute.For<IProviderDataService>();
        logger ??= Substitute.For<ILogger<ProviderNotificationEmailJob>>();

        return new ProviderNotificationEmailJob(
            providerDataService,
            logger);
    }
}
