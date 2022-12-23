using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Providers;
using StackExchange.Redis;

namespace Sfa.Tl.Find.Provider.Infrastructure.UnitTests.Builders;
public class RedisCacheServiceBuilder
{
    public RedisCacheService Build(
        IConnectionMultiplexer? connectionMultiplexer = null,
        IDateTimeProvider? dateTimeProvider = null,
        ILogger<RedisCacheService>? logger = null)
    {
        connectionMultiplexer ??= Substitute.For<IConnectionMultiplexer>();
        dateTimeProvider ??= new DateTimeProvider();
        logger ??= Substitute.For<ILogger<RedisCacheService>>();

        return new RedisCacheService(
            connectionMultiplexer,
            dateTimeProvider,
            logger);
    }
}
