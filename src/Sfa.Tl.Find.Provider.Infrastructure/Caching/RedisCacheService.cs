using System.Text.Json;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using StackExchange.Redis;

namespace Sfa.Tl.Find.Provider.Infrastructure.Caching;
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RedisCacheService(
        IConnectionMultiplexer connectionMultiplexer,
        IDateTimeProvider dateTimeProvider)
    {
        _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    public async Task<T?> Get<T>(string key)
    {
        key = CacheKeys.GenerateTypedCacheKey<T>(key);
        var database = GetDatabase();
        var cachedValue = await database.StringGetAsync(key);
        return cachedValue.HasValue ? JsonSerializer.Deserialize<T>(cachedValue!) : default;
    }

    public Task<bool> KeyExists<T>(string key)
    {
        key = CacheKeys.GenerateTypedCacheKey<T>(key);
        var database = GetDatabase();

        return database.KeyExistsAsync(key);
    }

    public async Task Set<T>(string key, T value, CacheDuration cacheDuration = CacheDuration.Standard)
    {
        await SetCustomValueAsync(key, value, TimeSpan.FromMinutes((int)cacheDuration));
    }

    public async Task Set<T>(string key, T value, DateTimeOffset absoluteExpiration)
    {
        await SetCustomValueAsync(key, value, absoluteExpiration - _dateTimeProvider.NowOffset);
    }

    public async Task Remove<T>(string key)
    {
        key = CacheKeys.GenerateTypedCacheKey<T>(key);
        var database = GetDatabase();
        await database.KeyDeleteAsync(key, CommandFlags.FireAndForget);
    }

    private async Task SetCustomValueAsync<T>(string key, T customType, TimeSpan cacheTime)
    {
        if (customType == null) return;

        key = CacheKeys.GenerateTypedCacheKey<T>(key);
        var database = GetDatabase();
        await database.StringSetAsync(key, JsonSerializer.Serialize(customType), cacheTime);
    }

    private IDatabase GetDatabase() => _connectionMultiplexer.GetDatabase();
}
