using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Providers;
using StackExchange.Redis;

namespace Sfa.Tl.Find.Provider.Infrastructure.UnitTests.Builders;
public class RedisCacheServiceBuilder
{
    public RedisCacheService Build(
        IConnectionMultiplexer? connectionMultiplexer = null,
        IDateTimeProvider? dateTimeProvider = null)
    {
        connectionMultiplexer ??= Substitute.For<IConnectionMultiplexer>();
        dateTimeProvider ??= new DateTimeProvider();

        return new RedisCacheService(
            connectionMultiplexer,
            dateTimeProvider);
    }
}
