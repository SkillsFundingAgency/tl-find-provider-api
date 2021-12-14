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

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args => args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information);

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args => args[2] != null && args[2].ToString() == "TestRepository saved TestData data. Inserted 3 rows. Updated 2 rows. Deleted 1 row.");
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

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args => args[2] != null && args[2].ToString() == "TestRepository saved TestData data. Updated 2 rows. Deleted 1 row.");
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

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args => args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information);

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args => args[2] != null && args[2].ToString() == "TestRepository saved TestData data. Inserted 3 rows. Deleted 1 row.");
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

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args => args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information);

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args => args[2] != null && args[2].ToString() == "TestRepository saved TestData data. Inserted 1 row. Updated 1 row.");
    }
}