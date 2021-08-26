using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Sfa.Tl.Find.Provider.Api.Extensions
{
    public static class CacheExtensions
    {
        public static void EvictionLoggingCallback(object key, object value, EvictionReason reason, object state)
        {
            var logger = state as ILogger;
            logger?.LogInformation($"Entry {key} was evicted from the cache. Reason: {reason}.");
        }
    }
}
