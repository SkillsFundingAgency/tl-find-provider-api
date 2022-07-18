using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Polly;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Extensions;

public class PollyContextExtensionsTests
{
    [Fact]
    public void PollyContextExtensions_TryGetLogger_Should_Return_Logger()
    {
        var loggerInContext = Substitute.For<ILogger>();
        var pollyContext = new Context("Test_Context",
            new Dictionary<string, object>
            {
                {
                    PolicyContextItems.Logger, loggerInContext
                }
            });

        var hasLogger = pollyContext.TryGetLogger(out var logger);

        hasLogger.Should().BeTrue();
        logger.Should().Be(loggerInContext);
    }

    [Fact]
    public void PollyContextExtensions_TryGetLogger_Should_Return_False_If_No_Logger()
    {
        var pollyContext = new Context("Test_Context");

        var hasLogger = pollyContext.TryGetLogger(out var logger);

        hasLogger.Should().BeFalse();
        logger.Should().BeNull();
    }
}