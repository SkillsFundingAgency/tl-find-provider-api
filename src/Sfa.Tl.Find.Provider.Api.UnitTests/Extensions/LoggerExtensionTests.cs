using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Extensions;

public class LoggerExtensionTests
{
    [Fact]
    public void LogChangeResults_Calls_Logger_With_Expected_Parameters()
    {
        var changeResult = new DataBaseChangeResultBuilder()
            .WithInserts(3)
            .WithUpdates(2)
            .WithDeletes(1)
            .Build();

        var logger = Substitute.For<ILogger<object>>();

        logger.LogChangeResults(changeResult, "TestRepository", "TestData");

        logger.ReceivedCalls().Count().Should().Be(4);

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args =>
                args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information &&
                args[2] != null && args[2].ToString() == "TestRepository saved TestData data.");

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args => 
                args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information &&
                args[2] != null && args[2].ToString() == " Inserted 3 row(s).");

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args => 
                args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information &&
                args[2] != null && args[2].ToString() == " Updated 2 row(s).");

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args => 
                args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information &&
                args[2] != null && args[2].ToString() == " Deleted 1 row(s).");
    }

    [Fact]
    public void LogChangeResults_With_No_Inserts_Calls_Logger_With_Expected_Parameters()
    {
        var changeResult = new DataBaseChangeResultBuilder()
            .WithUpdates(2)
            .WithDeletes(1)
            .Build();

        var logger = Substitute.For<ILogger<object>>();

        logger.LogChangeResults(changeResult, "TestRepository", "TestData", false);

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args => args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information);

        logger.ReceivedCalls().Count().Should().Be(3);
        
        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args => 
                args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information &&
                args[2] != null && args[2].ToString() == "TestRepository saved TestData data.");
        
        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args => 
                args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information &&
                args[2] != null && args[2].ToString() == " Updated 2 row(s).");
        
        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args =>
                args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information && 
                args[2] != null && args[2].ToString() == " Deleted 1 row(s).");
    }

    [Fact]
    public void LogChangeResults_With_No_Updates_Calls_Logger_With_Expected_Parameters()
    {
        var changeResult = new DataBaseChangeResultBuilder()
            .WithInserts(3)
            .WithDeletes(1)
            .Build();

        var logger = Substitute.For<ILogger<object>>();

        logger.LogChangeResults(changeResult, "TestRepository", "TestData", includeUpdated: false);

        logger.ReceivedCalls().Count().Should().Be(3);

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args =>
                args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information &&
                args[2] != null && args[2].ToString() == "TestRepository saved TestData data.");

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args =>
                args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information &&
                args[2] != null && args[2].ToString() == " Inserted 3 row(s).");
        
        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args =>
                args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information &&
                args[2] != null && args[2].ToString() == " Deleted 1 row(s).");
    }

    [Fact]
    public void LogChangeResults_With_NoDeletes_Calls_Logger_With_Expected_Parameters()
    {
        var changeResult = new DataBaseChangeResultBuilder()
            .WithInserts(1)
            .WithUpdates(1)
            .Build();

        var logger = Substitute.For<ILogger<object>>();

        logger.LogChangeResults(changeResult, "TestRepository", "TestData", includeDeleted: false);

        logger.ReceivedCalls().Count().Should().Be(3);

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args =>
                args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information &&
                args[2] != null && args[2].ToString() == "TestRepository saved TestData data.");

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args =>
                args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information &&
                args[2] != null && args[2].ToString() == " Inserted 1 row(s).");

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args =>
                args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information &&
                args[2] != null && args[2].ToString() == " Updated 1 row(s).");
    }
}