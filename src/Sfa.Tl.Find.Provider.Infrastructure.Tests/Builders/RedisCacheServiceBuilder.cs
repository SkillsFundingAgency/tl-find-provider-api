using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using StackExchange.Redis;

namespace Sfa.Tl.Find.Provider.Infrastructure.Tests.Builders;
public class RedisCacheServiceBuilder
{
    public RedisCacheService Build(
        IMemoryCache? cache = null,
        IConnectionMultiplexer? connectionMultiplexer = null,
        ILogger<RedisCacheService>? logger = null)
    {
        cache ??= Substitute.For<IMemoryCache>();
        connectionMultiplexer ??= Substitute.For<IConnectionMultiplexer>();
        logger ??= Substitute.For<ILogger<RedisCacheService>>();

        return new RedisCacheService(
            cache,
            connectionMultiplexer,
            logger);
    }
}
