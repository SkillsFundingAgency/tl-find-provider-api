﻿using Microsoft.Extensions.Caching.Memory;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using Sfa.Tl.Find.Provider.Infrastructure.Tests.Builders;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Infrastructure.Tests.Caching;
public class MemoryCacheServiceTests
{
    private const string TestKey = "key";
    private const string TestValue = "value";
    private const string FormattedStringTestKey = "key:string";

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
    public async Task Get_Calls_Inner_Cache_And_Returns_Expected_Result()
    {
        var memoryCache = Substitute.For<IMemoryCache>();
        memoryCache.TryGetValue(FormattedStringTestKey, out Arg.Any<string>())
            .Returns(x =>
            {
                x[1] = "value";
                return true;
            });

        var service = new MemoryCacheServiceBuilder().Build(memoryCache);

        var result = await service.Get<string>(TestKey);

        result.Should().BeEquivalentTo(TestValue);

        memoryCache
            .Received(1)
            .TryGetValue(FormattedStringTestKey, out Arg.Any<string>());
    }


    [Fact]
    public async Task KeyExists_Calls_Inner_Cache_And_Returns_Expected_Result()
    {
        var memoryCache = Substitute.For<IMemoryCache>();
        memoryCache.TryGetValue(FormattedStringTestKey, out Arg.Any<string>())
            .Returns(x =>
            {
                x[1] = "value";
                return true;
            });

        var service = new MemoryCacheServiceBuilder().Build(memoryCache);

        var result = await service.KeyExists<string>(TestKey);

        result.Should().BeTrue();

        memoryCache
            .Received(1)
            .TryGetValue(FormattedStringTestKey, out Arg.Any<string>());
    }

    [Fact]
    public async Task Set_Calls_Inner_Cache()
    {
        var memoryCache = Substitute.For<IMemoryCache>();

        var service = new MemoryCacheServiceBuilder().Build(memoryCache);

        await service.Set(TestKey, TestValue);

      memoryCache
            .Received(1)
            .CreateEntry(FormattedStringTestKey);
    }

    [Fact]
    public async Task Set_With_Duration_Calls_Inner_Cache()
    {
        const CacheDuration duration = CacheDuration.Medium;

        var memoryCache = Substitute.For<IMemoryCache>();

        var service = new MemoryCacheServiceBuilder().Build(memoryCache);

        await service.Set(TestKey, TestValue, duration);

        memoryCache
            .Received(1)
            .CreateEntry(FormattedStringTestKey);
    }

    [Fact]
    public async Task Set_With_Absolute_Expiration_Calls_Inner_Cache()
    {
        var expiration = new DateTimeOffset();

        var memoryCache = Substitute.For<IMemoryCache>();

        var service = new MemoryCacheServiceBuilder().Build(memoryCache);

        await service.Set(TestKey, TestValue, expiration);

        memoryCache
            .Received(1)
            .CreateEntry(FormattedStringTestKey);
    }

    [Fact]
    public async Task Remove_Calls_Inner_Cache()
    {
        var memoryCache = Substitute.For<IMemoryCache>();

        var service = new MemoryCacheServiceBuilder().Build(memoryCache);

        await service.Remove<string>(TestKey);

        memoryCache
            .Received(1)
            .Remove(FormattedStringTestKey);
    }
}
