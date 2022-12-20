using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Caching.Memory;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using Sfa.Tl.Find.Provider.Infrastructure.Tests.Builders;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using StackExchange.Redis;

namespace Sfa.Tl.Find.Provider.Infrastructure.Tests.Caching;
public class RedisCacheServiceTests
{
    private const string TestKey = "key";
    private const string TestValue = "value";
    private const string FormattedStringTestKey = "key:string";

    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(RedisCacheService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(RedisCacheService)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }


    [Fact]
    public async Task Remove_Calls_Redis_Database()
    {
        var database = Substitute.For<IDatabase>();
        var connectionMultiplexer = Substitute.For<IConnectionMultiplexer>();
        connectionMultiplexer
            .GetDatabase()
            .Returns(database);

        var service = new RedisCacheServiceBuilder().Build(connectionMultiplexer);
        await service.Remove<string>(TestKey);

        await database
            .Received(1)
            .KeyDeleteAsync(FormattedStringTestKey, CommandFlags.FireAndForget);
    }
}
