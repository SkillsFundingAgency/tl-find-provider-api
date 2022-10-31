using Microsoft.Extensions.Caching.Memory;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using Sfa.Tl.Find.Provider.Infrastructure.Tests.Builders;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Infrastructure.Tests;
public class MemoryCacheServiceTests
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
    
    [Fact]
    public void Get_Calls_Inner_Cache_And_Returns_Expected_Result()
    {
        var memoryCache = Substitute.For<IMemoryCache>();
        memoryCache.TryGetValue(TestKey, out Arg.Any<string>())
            .Returns(x =>
            {
                x[1] = "value";
                return true;
            });
            
        var service = new MemoryCacheServiceBuilder().Build(memoryCache);

        var result = service.TryGetValue(TestKey, out string returnedValue);

        result.Should().BeTrue();
        returnedValue.Should().BeEquivalentTo(TestValue);

        memoryCache
            .Received(1)
            .TryGetValue(Arg.Any<string>(), out Arg.Any<string>());
        memoryCache
            .Received(1)
            .TryGetValue(TestKey, out Arg.Any<string>());
        memoryCache
            .Received(1)
            .TryGetValue(TestKey, out returnedValue);
    }

    [Fact]
    public void Set_Calls_Inner_Cache()
    {
        var memoryCache = Substitute.For<IMemoryCache>();

        var service = new MemoryCacheServiceBuilder().Build(memoryCache);

        var result = service.Set(TestKey, TestValue);

        result.Should().Be(TestValue);

        memoryCache
            .Received(1)
            .CreateEntry(Arg.Any<string>());
        memoryCache
            .Received(1)
            .CreateEntry(Arg.Is<string>(k => k == TestKey));
    }


    [Fact]
    public void Remove_Calls_Inner_Cache()
    {
        var memoryCache = Substitute.For<IMemoryCache>();

        var service = new MemoryCacheServiceBuilder().Build(memoryCache);

        service.Remove(TestKey);

        memoryCache
            .Received(1)
            .Remove(Arg.Any<string>());
        memoryCache
            .Received(1)
            .Remove(TestKey);
    }
}
