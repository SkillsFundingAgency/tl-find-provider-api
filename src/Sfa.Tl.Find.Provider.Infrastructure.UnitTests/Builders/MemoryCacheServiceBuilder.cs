using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;

namespace Sfa.Tl.Find.Provider.Infrastructure.UnitTests.Builders;
public class MemoryCacheServiceBuilder
{
    public MemoryCacheService Build(
        IMemoryCache? cache = null,
        ILogger<MemoryCacheService>? logger = null)
    {
        cache ??= Substitute.For<IMemoryCache>();
        logger ??= Substitute.For<ILogger<MemoryCacheService>>();

        return new MemoryCacheService(
            cache,
            logger);
    }
}
