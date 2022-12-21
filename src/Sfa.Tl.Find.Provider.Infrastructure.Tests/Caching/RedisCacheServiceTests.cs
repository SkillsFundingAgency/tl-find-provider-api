using System.Text.Json;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using Sfa.Tl.Find.Provider.Infrastructure.Tests.Builders;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using StackExchange.Redis;

namespace Sfa.Tl.Find.Provider.Infrastructure.Tests.Caching;
public class RedisCacheServiceTests
{
    private const string MissingKey = "non_existent_key";
    private const string TestKey = "key";
    private const string TestValue = "value";
    private const string FormattedStringMissingKey = $"{MissingKey}:string";
    private const string FormattedStringTestKey = $"{TestKey}:string";

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
    public async Task Get_Calls_Redis_Database_And_Returns_Expected_Result_For_Valid_Key()
    {
        var (connectionMultiplexer, database) = CreateSubstituteConnectionMultiplexerAndDatabase();

        var service = new RedisCacheServiceBuilder().Build(connectionMultiplexer);

        var result = await service.Get<string>(TestKey);

        result.Should().BeEquivalentTo(TestValue);

        await database
            .Received(1)
            .StringGetAsync(FormattedStringTestKey);
    }

    [Fact]
    public async Task Get_Calls_Redis_Database_And_Returns_Expected_Result_For_Missing_Key()
    {
        var (connectionMultiplexer, database) = CreateSubstituteConnectionMultiplexerAndDatabase();

        var service = new RedisCacheServiceBuilder().Build(connectionMultiplexer);

        var result = await service.Get<string>(MissingKey);

        result.Should().Be(null);

        await database
            .Received(1)
            .StringGetAsync(FormattedStringMissingKey);
    }

    [Fact]
    public async Task KeyExists_Calls_Redis_Database_And_Returns_Expected_Result_For_Valid_Key()
    {
        var (connectionMultiplexer, database) = CreateSubstituteConnectionMultiplexerAndDatabase();

        var service = new RedisCacheServiceBuilder().Build(connectionMultiplexer);

        var result = await service.KeyExists<string>(TestKey);

        result.Should().BeTrue();

        await database
            .Received(1)
            .KeyExistsAsync(FormattedStringTestKey);
    }

    [Fact]
    public async Task KeyExists_Calls_Redis_Database_And_Returns_Expected_Result_For_Missing_Key()
    {
        var (connectionMultiplexer, database) = CreateSubstituteConnectionMultiplexerAndDatabase();

        var service = new RedisCacheServiceBuilder().Build(connectionMultiplexer);

        var result = await service.KeyExists<string>(MissingKey);

        result.Should().BeFalse();

        await database
            .Received(1)
            .KeyExistsAsync(FormattedStringMissingKey);
    }

    [Fact]
    public async Task Set_Calls_Redis_Database_With_Default_Cache_Duration()
    {
        var serializedValue = JsonSerializer.Serialize(TestValue);
        var expectedCacheDuration = TimeSpan.FromMinutes((int)CacheDuration.Standard);

        var (connectionMultiplexer, database) = CreateSubstituteConnectionMultiplexerAndDatabase();

        var service = new RedisCacheServiceBuilder().Build(connectionMultiplexer);
        await service.Set(TestKey, TestValue);

        await database
               .Received(1)
               .StringSetAsync(FormattedStringTestKey, serializedValue, expectedCacheDuration);
    }

    [Fact]
    public async Task Set_Calls_Redis_Database_With_Cache_Duration()
    {
        var serializedValue = JsonSerializer.Serialize(TestValue);
        const CacheDuration cacheDuration = CacheDuration.Medium;
        var expectedCacheDuration = TimeSpan.FromMinutes((int)cacheDuration);

        var (connectionMultiplexer, database) = CreateSubstituteConnectionMultiplexerAndDatabase();

        var service = new RedisCacheServiceBuilder().Build(connectionMultiplexer);
        await service.Set(TestKey, TestValue, cacheDuration);

        await database
            .Received(1)
            .StringSetAsync(FormattedStringTestKey, serializedValue, expectedCacheDuration);
    }

    [Fact]
    public async Task Set_With_Absolute_Expiration_Calls_Inner_Cache()
    {
        var serializedValue = JsonSerializer.Serialize(TestValue);
        const short offset = 10;
        var expiration = DateTimeOffset.UtcNow.AddMinutes(offset);

        var expectedCacheDuration = expiration - DateTimeOffset.Now;

        //var expectedCacheDuration = TimeSpan.FromMinutes((int)cacheDuration);

        var (connectionMultiplexer, database) = CreateSubstituteConnectionMultiplexerAndDatabase();

        var service = new RedisCacheServiceBuilder().Build(connectionMultiplexer);

        await service.Set(TestKey, TestValue, expiration);

        await database
            .Received(1)
            //.StringSetAsync(FormattedStringTestKey, serializedValue, expectedCacheDuration);
            .StringSetAsync(FormattedStringTestKey, serializedValue, Arg.Any<TimeSpan>());
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

    private static (IConnectionMultiplexer ConnectionMultiplexer, IDatabase Database)
        CreateSubstituteConnectionMultiplexerAndDatabase(
            string key = FormattedStringTestKey,
            string value = TestValue)
    {
        var serializedValue = JsonSerializer.Serialize(value);

        var database = Substitute.For<IDatabase>();
        database
            .KeyExistsAsync(Arg.Any<RedisKey>())
            .Returns(k => (RedisKey)k[0] == key);
        database
            .StringGetAsync(Arg.Any<RedisKey>())
            .Returns(k =>
                (RedisKey)k[0] == key
                    ? serializedValue
                    : default(RedisValue));

        var connectionMultiplexer = Substitute.For<IConnectionMultiplexer>();
        connectionMultiplexer
            .GetDatabase()
            .Returns(database);

        return (connectionMultiplexer, database);
    }
}
