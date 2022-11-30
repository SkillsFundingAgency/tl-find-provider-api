using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Infrastructure.Tests.Extensions;

public class CacheUtilitiesTests
{
    [Fact]
    public void DefaultMemoryCacheEntryOptions_Returns_Expected_Value()
    {
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        var logger = Substitute.For<ILogger<object>>();

        var options = CacheUtilities.DefaultMemoryCacheEntryOptions(
            dateTimeProvider,
            logger);

        options.Should().NotBeNull();
    }

    [Fact]
    public void EvictionLoggingCallback_Ignores_Null_Logger()
    {
        const string key = "TEST_KEY";
        const EvictionReason reason = EvictionReason.Removed;
        var value = new { x = "test " };

        var act = () => CacheUtilities.EvictionLoggingCallback(key, value, reason, null);
        act
            .Should().NotThrow<ArgumentNullException>();
    }

    [Fact]
    public void EvictionLoggingCallback_Calls_Logger()
    {
        const string key = "TEST_KEY";
        const EvictionReason reason = EvictionReason.Removed;
        var value = new { x = "test " };
        var logger = Substitute.For<ILogger<object>>();

        CacheUtilities.EvictionLoggingCallback(key, value, reason, logger);
            
        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args => args[0] is LogLevel && (LogLevel)args[0]! == LogLevel.Information);
            
        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args => args[2] != null && args[2]!.ToString() == $"Entry {key} was evicted from the cache. Reason: {reason}.");
    }
}