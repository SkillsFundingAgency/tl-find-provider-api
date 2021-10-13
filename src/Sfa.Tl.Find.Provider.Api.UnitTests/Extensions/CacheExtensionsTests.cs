using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Xunit;
using CacheExtensions = Sfa.Tl.Find.Provider.Api.Extensions.CacheExtensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Extensions
{
    public class CacheExtensionsTests
    {
        [Fact]
        public void DefaultMemoryCacheEntryOptions_Returns_Expected_Value()
        {
            var dateTimeService = Substitute.For<IDateTimeService>();
            var logger = Substitute.For<ILogger<object>>();

            var options = CacheExtensions.DefaultMemoryCacheEntryOptions(
                dateTimeService,
                logger);

            options.Should().NotBeNull();
        }

        [Fact]
        public void EvictionLoggingCallback_Ignores_Null_Logger()
        {
            const string key = "TEST_KEY";
            const EvictionReason reason = EvictionReason.Removed;
            var value = new { x = "test " };

            Action act = () => CacheExtensions.EvictionLoggingCallback(key, value, reason, null);
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

            CacheExtensions.EvictionLoggingCallback(key, value, reason, logger);
            
            logger.ReceivedCalls()
                .Select(call => call.GetArguments())
                .Should()
                .Contain(args => args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information);
            
            logger.ReceivedCalls()
                .Select(call => call.GetArguments())
                .Should()
                .Contain(args => args[2] != null && args[2].ToString() == $"Entry {key} was evicted from the cache. Reason: {reason}.");
        }
    }
}
