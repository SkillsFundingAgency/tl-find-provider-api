using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Infrastructure.Caching;
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryCacheService> _logger;

    public MemoryCacheService(
        IMemoryCache cache,
        ILogger<MemoryCacheService> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<T?> Get<T>(string key)
    {
        key = CacheKeys.GenerateTypedCacheKey<T>(key);
        return _cache.TryGetValue<string>(key, out var value) 
            ? Task.FromResult(JsonSerializer.Deserialize<T>(value)) 
            : Task.FromResult(default(T));
    }

    public Task<bool> KeyExists<T>(string key)
    {
        key = CacheKeys.GenerateTypedCacheKey<T>(key);
        return Task.FromResult(_cache.TryGetValue<string>(key, out _));
    }

    public Task Set<T>(string key, T value, CacheDuration cacheDuration = CacheDuration.Standard)
    {
        key = CacheKeys.GenerateTypedCacheKey<T>(key);
        _cache.Set(key, JsonSerializer.Serialize(value), TimeSpan.FromMinutes((int)cacheDuration));
        return Task.CompletedTask;
    }

    public Task Set<T>(string key, T value, DateTimeOffset absoluteExpiration)
    {
        key = CacheKeys.GenerateTypedCacheKey<T>(key);
        _cache.Set(key, JsonSerializer.Serialize(value), absoluteExpiration);
        return Task.CompletedTask;
    }

    public Task Remove<T>(string key)
    {
        key = CacheKeys.GenerateTypedCacheKey<T>(key);
        _cache.Remove(key);

        return Task.CompletedTask;
    }
}
