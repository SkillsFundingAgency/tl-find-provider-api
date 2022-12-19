using Microsoft.Extensions.Caching.Memory;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;

namespace Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
public interface ICacheService
{
    Task<T?> Get<T>(string key);

    Task<bool> KeyExists<T>(string key);

    Task Set<T>(string key, T item, CacheDuration cacheDuration = CacheDuration.Standard);

    Task Set<T>(string key, T value, DateTimeOffset absoluteExpiration);

    Task Remove<T>(string key);
}
