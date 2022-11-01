using Microsoft.Extensions.Caching.Memory;

namespace Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
public interface ICacheService
{
    T Get<T>(object key);

    bool TryGetValue<T>(object key, out T value);

    T Set<T>(string key, T value);

    T Set<T>(string key, T value, DateTimeOffset absoluteExpiration);
    
    T Set<T>(string key, T value, MemoryCacheEntryOptions options);

    void Remove(object key);
}
