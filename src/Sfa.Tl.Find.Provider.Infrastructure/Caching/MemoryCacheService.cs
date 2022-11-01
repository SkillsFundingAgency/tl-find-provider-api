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
}
