using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Extensions
{
    public static class CacheExtensions
    {
        public static MemoryCacheEntryOptions DefaultMemoryCacheEntryOptions(
            IDateTimeService dateTimeService,
            ILogger logger,
            double absoluteExpirationInMinutes = 60,
            double slidingExpirationInMinutes = 5,
            int size = 1) =>
            new()
            {
                AbsoluteExpiration = absoluteExpirationInMinutes > 0 
                    ? new DateTimeOffset(dateTimeService.Now.AddMinutes(absoluteExpirationInMinutes)) 
                    : null,
                Priority = CacheItemPriority.Normal,
                SlidingExpiration = slidingExpirationInMinutes > 0 
                    ? TimeSpan.FromMinutes(slidingExpirationInMinutes) 
                    : null,
                Size = size,
                PostEvictionCallbacks =
                {
                    new PostEvictionCallbackRegistration
                    {
                        EvictionCallback = EvictionLoggingCallback,
                        State = logger
                    }
                }
            };

        public static void EvictionLoggingCallback(object key, object value, EvictionReason reason, object state)
        {
            var logger = state as ILogger;
            logger?.LogInformation($"Entry {key} was evicted from the cache. Reason: {reason}.");
        }
    }
}
