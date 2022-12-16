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

    public T Get<T>(object key)
    {
        TryGetValue(key, out T value);
        return value;
    }

    public bool TryGetValue<T>(object key, out T value)
    {
        return _cache.TryGetValue(key, out value);
    }

    public T Set<T>(string key, T value)
    {
        return _cache.Set(key, value);
    }

    public T Set<T>(string key, T value, DateTimeOffset absoluteExpiration)
    {
        return _cache.Set(key, value, absoluteExpiration);
    }

    public T Set<T>(string key, T value, MemoryCacheEntryOptions options)
    {
        return _cache.Set(key, value, options);
    }

    public void Remove(object key)
    {
        _cache.Remove(key);
    }

    //    public T Get<T>(object key)
    //    {
    //        return GetAsync<T>((string)key).GetAwaiter().GetResult();
    //    }

    //    public bool TryGetValue<T>(object key, out T value)
    //    {
    //        return _cache.TryGetValue(key, out value);
    //    }

    //    public T Set<T>(string key, T value)
    //    {
    //        return SetAsync<T>((string)key, value).GetAwaiter().GetResult();
    //    }

    //    public T Set<T>(string key, T value, DateTimeOffset absoluteExpiration)
    //    {
    //        return _cache.Set(key, value, absoluteExpiration);
    //    }

    //    public T Set<T>(string key, T value, MemoryCacheEntryOptions options)
    //    {
    //        return _cache.Set(key, value, options);
    //    }

    //    public void Remove(object key)
    //    {
    //        RemoveAsync<T>(key.ToString()).GetAwaiter().GetResult();
    //    }

    //    public async Task<T> GetAsync<T>(string key)
    //    {
    //        var database = GetDatabase();
    //        var cachedValue = await database.StringGetAsync(key);
    //        return cachedValue.HasValue ? JsonConvert.DeserializeObject<T>(cachedValue) : default(T);
    //    }

    //    public Task<bool> TryGetValueAsync<T>(string key)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public async Task SetAsync<T>(string key, T item, CacheDuration cacheDuration = CacheDuration.Standard)
    //    {
    //        await SetCustomValueAsync(key, item, TimeSpan.FromMinutes((int)cacheDuration));
    //    }

    //    public Task SetAsync<T>(string key, T value, DateTimeOffset absoluteExpiration)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task RemoveAsync<T>(string key)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private async Task SetCustomValueAsync<T>(string key, T customType, TimeSpan cacheTime)
    //    {
    //        if (customType == null) return;
    //        var database = GetDatabase();
    //        await database.StringSetAsync(key, JsonConvert.SerializeObject(customType), cacheTime);
    //    }

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
