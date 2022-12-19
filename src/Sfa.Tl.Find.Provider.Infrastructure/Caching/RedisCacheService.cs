using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using StackExchange.Redis;

namespace Sfa.Tl.Find.Provider.Infrastructure.Caching;
public class RedisCacheService : ICacheService, IDisposable
{
    private readonly IMemoryCache _cache;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(
        IMemoryCache cache,
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<RedisCacheService> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _connectionMultiplexer = connectionMultiplexer;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool TryGetValue<T>(object key, out T value)
    {
        throw new NotImplementedException();
    }

    public async Task<T?> Get<T>(string key)
    {
        var database = GetDatabase();
        var cachedValue = await database.StringGetAsync(key);
        return cachedValue.HasValue ? JsonSerializer.Deserialize<T>(cachedValue) : default(T);
    }

    public Task<bool> TryGetValueAsync<T>(string key, out T value)
    {
        throw new NotImplementedException();
    }

    public Task<bool> KeyExists<T>(string key)
    {
        key = GenerateCacheKey<T>(key);

        var database = GetDatabase();

        return database.KeyExistsAsync(key);
    }

    public async Task Set<T>(string key, T item, CacheDuration cacheDuration = CacheDuration.Standard)
    {
        await SetCustomValueAsync(key, item, TimeSpan.FromMinutes((int)cacheDuration));
    }

    public async Task Set<T>(string key, T value, DateTimeOffset absoluteExpiration)
    {
        throw new NotImplementedException();
    }

    public Task Remove<T>(string key)
    {
        throw new NotImplementedException();
    }

    private async Task SetCustomValueAsync<T>(string key, T customType, TimeSpan cacheTime)
    {
        if (customType == null) return;
        var database = GetDatabase();
        await database.StringSetAsync(key, JsonSerializer.Serialize(customType), cacheTime);
    }

    //    public async Task RemoveAsync(string key)
    //    {
    //        var database = GetDatabase();
    //        await database.KeyDeleteAsync(key, CommandFlags.FireAndForget);
    //    }

    //    //public async Task<T> GetAndRemoveAsync<T>(string key)
    //    //{
    //    //    var database = GetDatabase();
    //    //    var cachedValue = await database.StringGetAsync(key);
    //    //    await database.KeyDeleteAsync(key, CommandFlags.FireAndForget);
    //    //    return cachedValue.HasValue ? JsonConvert.DeserializeObject<T>(cachedValue) : default(T);
    //    //}

    private IDatabase GetDatabase() => _connectionMultiplexer.GetDatabase();

    private static string GenerateCacheKey<T>(string key)
    {
        return GenerateCacheKey(typeof(T), key);
    }

    private static string GenerateCacheKey(Type objectType, string key)
    {
        return $"{key}:{objectType.Name}".ToLower();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _connectionMultiplexer.Dispose();
    }
}
