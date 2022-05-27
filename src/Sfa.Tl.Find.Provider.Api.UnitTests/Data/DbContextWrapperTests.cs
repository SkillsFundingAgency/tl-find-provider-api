using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Polly;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Policies;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Data;

public class DbContextWrapperTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(DbContextWrapper)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void CreateConnection_Returns_Expected_Connection()
    {
        var dbContextWrapper = new DbContextWrapperBuilder().Build();

        var connection = dbContextWrapper.CreateConnection();
        connection.Should().NotBeNull();
    }

    [Fact]
    public void BeginTransaction_Calls_Begin_Transaction_Method_On_Connection()
    {
        var dbContextWrapper = new DbContextWrapperBuilder().Build();

        var connection = Substitute.For<IDbConnection>();
        dbContextWrapper.BeginTransaction(connection);

        connection.Received(1).BeginTransaction();
    }

    [Fact]
    public async Task Query_Async_T_Calls_Retry_Policy_With_Logger()
    {
        var logger = Substitute.For<ILogger<DbContextWrapper>>();

        var (policy, policyRegistry) = PollyPolicyBuilder.BuildPolicyAndRegistry();

        var dbContextWrapper = new DbContextWrapperBuilder()
            .Build(policyRegistry: policyRegistry,
                   logger: logger);

        const string sql = "SELECT * FROM Qualification";

        var connection = Substitute.For<IDbConnection>();

        var _ = await dbContextWrapper.QueryAsync<Qualification>(
            connection,
            sql);

        await policy.Received(1).ExecuteAsync(
            Arg.Any<Func<Context, Task<IEnumerable<Qualification>>>>(),
            Arg.Is<Context>(ctx =>
                ctx.ContainsKey(PolicyContextItems.Logger) &&
                    ctx[PolicyContextItems.Logger] == logger
            ));
    }

    [Fact]
    public async Task QueryAsync_TFirst_TSecond_Calls_Retry_Policy_With_Logger()
    {
        var logger = Substitute.For<ILogger<DbContextWrapper>>();

        var (policy, policyRegistry) = PollyPolicyBuilder.BuildPolicyAndRegistry();

        var dbContextWrapper = new DbContextWrapperBuilder()
            .Build(policyRegistry: policyRegistry,
                logger: logger);

        const string sql = "SELECT * FROM Qualification";

        var connection = Substitute.For<IDbConnection>();

        var map = Substitute.For<Func<object, object, Qualification>>();

        var _ = await dbContextWrapper.QueryAsync<Qualification>(
            connection,
            sql,
            map);

        await policy.Received(1).ExecuteAsync(
            Arg.Any<Func<Context, Task<IEnumerable<Qualification>>>>(),
            Arg.Is<Context>(ctx =>
                ctx.ContainsKey(PolicyContextItems.Logger) &&
                ctx[PolicyContextItems.Logger] == logger
            ));
    }
    
    [Fact]
    public async Task QueryAsync_TFirst_TSecond_TThird_TFourth_Calls_Retry_Policy_With_Logger()
    {
        var logger = Substitute.For<ILogger<DbContextWrapper>>();

        var (policy, policyRegistry) = PollyPolicyBuilder.BuildPolicyAndRegistry();

        var dbContextWrapper = new DbContextWrapperBuilder()
            .Build(policyRegistry: policyRegistry,
                logger: logger);

        const string sql = "SELECT * FROM Qualification";

        var connection = Substitute.For<IDbConnection>();

        var map = Substitute.For<Func<object, object, object, object, Qualification>>();

        var _ = await dbContextWrapper.QueryAsync<Qualification>(
            connection,
            sql,
            map);

        await policy.Received(1).ExecuteAsync(
            Arg.Any<Func<Context, Task<IEnumerable<Qualification>>>>(),
            Arg.Is<Context>(ctx =>
                ctx.ContainsKey(PolicyContextItems.Logger) &&
                ctx[PolicyContextItems.Logger] == logger
            ));
    }

    [Fact]
    public async Task ExecuteScalarAsync_Calls_Retry_Policy_With_Logger()
    {
        var logger = Substitute.For<ILogger<DbContextWrapper>>();

        var (policy, policyRegistry) = PollyPolicyBuilder.BuildPolicyAndRegistry();

        var dbContextWrapper = new DbContextWrapperBuilder()
            .Build(policyRegistry: policyRegistry,
                logger: logger);

        const string sql = "SELECT * FROM Qualification";

        var connection = Substitute.For<IDbConnection>();

        var _ = await dbContextWrapper.ExecuteScalarAsync<int>(
            connection,
            sql);

        await policy.Received(1).ExecuteAsync(
            Arg.Any<Func<Context, Task<int>>>(),
            Arg.Any<Context>());

        await policy.Received(1).ExecuteAsync(
            Arg.Any<Func<Context, Task<int>>>(),
            Arg.Is<Context>(ctx =>
                ctx.ContainsKey(PolicyContextItems.Logger) &&
                ctx[PolicyContextItems.Logger] == logger
            ));
    }
}