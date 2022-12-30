using Microsoft.Extensions.Caching.Memory;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;

namespace Sfa.Tl.Find.Provider.Infrastructure.UnitTests.Builders;
public class MemoryCacheServiceBuilder
{
    public MemoryCacheService Build(
        IMemoryCache? cache = null)
    {
        cache ??= Substitute.For<IMemoryCache>();
     
        return new MemoryCacheService(cache);
    }
}
