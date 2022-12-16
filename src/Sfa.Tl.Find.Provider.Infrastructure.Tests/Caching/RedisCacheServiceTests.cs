using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using Sfa.Tl.Find.Provider.Infrastructure.Tests.Builders;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Infrastructure.Tests.Caching;
public class RedisCacheServiceTests
{
    private const string TestKey = "key";
    private const string TestValue = "value";

    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(MemoryCacheService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(MemoryCacheService)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }
}
