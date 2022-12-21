using System.Text.Json;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using StackExchange.Redis;

namespace Sfa.Tl.Find.Provider.Infrastructure.Caching;
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<RedisCacheService> logger)
    {
        _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer)); ;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<T?> Get<T>(string key)
    {
        key = GenerateCacheKey<T>(key);
        _logger.LogInformation("RedisCacheService::Get {key} of type {type}", key, typeof(T).Name);

        var database = GetDatabase();
        var cachedValue = await database.StringGetAsync(key);
        return cachedValue.HasValue ? JsonSerializer.Deserialize<T>(cachedValue) : default(T);
    }

    public Task<bool> KeyExists<T>(string key)
    {
        key = GenerateCacheKey<T>(key);
        _logger.LogInformation("RedisCacheService::KeyExists {key} of type {type}", key, typeof(T).Name);

        var database = GetDatabase();

        return database.KeyExistsAsync(key);
    }

    public async Task Set<T>(string key, T value, CacheDuration cacheDuration = CacheDuration.Standard)
    {
        await SetCustomValueAsync(key, value, TimeSpan.FromMinutes((int)cacheDuration));
    }

    public async Task Set<T>(string key, T value, DateTimeOffset absoluteExpiration)
    {
        var cacheDuration = absoluteExpiration - DateTimeOffset.Now;

        await SetCustomValueAsync(key, value, cacheDuration);
    }

    public async Task Remove<T>(string key)
    {
        key = GenerateCacheKey<T>(key);
        _logger.LogInformation("RedisCacheService::Remove {key} of type {type}", key, typeof(T).Name);

        var database = GetDatabase();
        await database.KeyDeleteAsync(key, CommandFlags.FireAndForget);
    }

    private async Task SetCustomValueAsync<T>(string key, T customType, TimeSpan cacheTime)
    {
        if (customType == null) return;

        key = GenerateCacheKey<T>(key);
        _logger.LogInformation("RedisCacheService::SetCustomValueAsync {key} of type {type}", key, typeof(T).Name);

        var database = GetDatabase();
        await database.StringSetAsync(key, JsonSerializer.Serialize(customType), cacheTime);
    }

    private IDatabase GetDatabase() => _connectionMultiplexer.GetDatabase();

    private static string GenerateCacheKey<T>(string key)
    {
        return GenerateCacheKey(typeof(T), key);
    }

    private static string GenerateCacheKey(Type objectType, string key)
    {
        return $"{key}:{objectType.Name}".ToLower();
    }
}
