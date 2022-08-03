using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Application.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Extensions;

public class LoggerExtensionTests
{
    [Fact]
    public void LogChangeResults_Calls_Logger_With_Expected_Parameters()
    {
        const int insertCount = 3;
        const int updateCount = 2;
        const int deleteCount = 1;

        var changeResult = new DataBaseChangeResultBuilder()
            .WithInserts(insertCount)
            .WithUpdates(updateCount)
            .WithDeletes(deleteCount)
            .Build();

        var expected = "TestRepository saved TestData data. " +
            $"Inserted {insertCount} row(s). " +
            $"Updated {updateCount} row(s). " +
            $"Deleted {deleteCount} row(s).";

        var logger = Substitute.For<ILogger<object>>();

        logger.LogChangeResults(changeResult, "TestRepository", "TestData");

        logger.ReceivedCalls().Count().Should().Be(1);
        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args =>
                args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information &&
                args[2] != null && args[2].ToString() == expected);
    }

    [Fact]
    public void LogChangeResults_With_No_Inserts_Calls_Logger_With_Expected_Parameters()
    {
        const int updateCount = 2;
        const int deleteCount = 1;

        var expected = "TestRepository saved TestData data. " +
            $"Updated {updateCount} row(s). " +
            $"Deleted {deleteCount} row(s).";

        var changeResult = new DataBaseChangeResultBuilder()
            .WithUpdates(updateCount)
            .WithDeletes(deleteCount)
            .Build();

        var logger = Substitute.For<ILogger<object>>();

        logger.LogChangeResults(changeResult, "TestRepository", "TestData", false);

        logger.ReceivedCalls().Count().Should().Be(1);
        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args =>
                args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information &&
                args[2] != null && args[2].ToString() == expected);
    }

    [Fact]
    public void LogChangeResults_With_No_Updates_Calls_Logger_With_Expected_Parameters()
    {
        const int insertCount = 3;
        const int deleteCount = 1;

        var changeResult = new DataBaseChangeResultBuilder()
            .WithInserts(insertCount)
            .WithDeletes(deleteCount)
            .Build();

        var expected = "TestRepository saved TestData data. " +
            $"Inserted {insertCount} row(s). " +
            $"Deleted {deleteCount} row(s).";

        var logger = Substitute.For<ILogger<object>>();

        logger.LogChangeResults(changeResult, "TestRepository", "TestData", includeUpdated: false);

        logger.ReceivedCalls().Count().Should().Be(1);
        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args =>
                args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information &&
                args[2] != null && args[2].ToString() == expected);
    }

    [Fact]
    public void LogChangeResults_With_NoDeletes_Calls_Logger_With_Expected_Parameters()
    {
        const int insertCount = 1;
        const int updateCount = 1;

        var changeResult = new DataBaseChangeResultBuilder()
            .WithInserts(insertCount)
            .WithUpdates(updateCount)
            .Build();

        var expected = "TestRepository saved TestData data. " +
            $"Inserted {insertCount} row(s). " +
            $"Updated {updateCount} row(s).";

        var logger = Substitute.For<ILogger<object>>();

        logger.LogChangeResults(changeResult, "TestRepository", "TestData", includeDeleted: false);

        logger.ReceivedCalls().Count().Should().Be(1);
        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args =>
                args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information &&
                args[2] != null && args[2].ToString() == expected);
    }
}