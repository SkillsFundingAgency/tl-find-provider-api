using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Infrastructure.Extensions;

public static class CacheUtilities
{
    public static MemoryCacheEntryOptions DefaultMemoryCacheEntryOptions(
        IDateTimeProvider dateTimeProvider,
        ILogger logger,
        int absoluteExpirationInMinutes = Constants.DefaultAbsoluteExpirationInMinutes,
        int slidingExpirationInMinutes = Constants.DefaultSlidingExpirationInMinutes,
        int size = 1) =>
        new()
        {
            AbsoluteExpiration = absoluteExpirationInMinutes > 0 
                ? new DateTimeOffset(dateTimeProvider.Now.AddMinutes(absoluteExpirationInMinutes)) 
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

    public static void EvictionLoggingCallback(object key, object value, EvictionReason reason, object? state)
    {
        if (state is ILogger logger && 
            logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug($"Entry {key} was evicted from the cache. Reason: {reason}.");
        }
    }
}