using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;

public class NotificationRepositoryBuilder
{
    public INotificationRepository Build(
        IDbContextWrapper dbContextWrapper = null,
        IDynamicParametersWrapper dynamicParametersWrapper = null,
        ILogger<NotificationRepository> logger = null)
    {
        dbContextWrapper ??= Substitute.For<IDbContextWrapper>();
        dynamicParametersWrapper ??= Substitute.For<IDynamicParametersWrapper>();
        logger ??= Substitute.For<ILogger<NotificationRepository>>();

        return new NotificationRepository(
            dbContextWrapper,
            dynamicParametersWrapper,
            logger);
    }
}