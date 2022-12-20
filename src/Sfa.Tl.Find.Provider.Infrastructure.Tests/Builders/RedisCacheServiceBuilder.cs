using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using StackExchange.Redis;

namespace Sfa.Tl.Find.Provider.Infrastructure.Tests.Builders;
public class RedisCacheServiceBuilder
{
    public RedisCacheService Build(
        IConnectionMultiplexer? connectionMultiplexer = null,
        ILogger<RedisCacheService>? logger = null)
    {
        connectionMultiplexer ??= Substitute.For<IConnectionMultiplexer>();
        logger ??= Substitute.For<ILogger<RedisCacheService>>();

        return new RedisCacheService(
            connectionMultiplexer,
            logger);
    }
}
