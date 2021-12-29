﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Polly;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Data;

public class QualificationRepositoryTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(QualificationRepository)
            .ShouldNotAcceptNullConstructorArguments();
    }
        
    [Fact]
    public async Task GetAll_Returns_Expected_List()
    {
        var qualifications = new QualificationBuilder()
            .BuildList()
            .ToList();

        var dbConnection = Substitute.For<IDbConnection>();
        var dbContextWrapper = Substitute.For<IDbContextWrapper>();
        dbContextWrapper
            .CreateConnection()
            .Returns(dbConnection);
        dbContextWrapper
            .QueryAsync<Qualification>(dbConnection, Arg.Any<string>())
            .Returns(qualifications);

        var repository = new QualificationRepositoryBuilder().Build(dbContextWrapper);

        var results = (await repository.GetAll()).ToList();
        results.Should().NotBeNullOrEmpty();
        results.Count.Should().Be(qualifications.Count);
    }

    [Fact]
    public async Task HasAny_Returns_False_When_Zero_Rows_Exist()
    {
        var dbConnection = Substitute.For<IDbConnection>();
        var dbContextWrapper = Substitute.For<IDbContextWrapper>();
        dbContextWrapper
            .CreateConnection()
            .Returns(dbConnection);
        dbContextWrapper
            .ExecuteScalarAsync<int>(dbConnection,
                Arg.Is<string>(s => s.Contains("dbo.Qualification")))
            .Returns(0);

        var repository = new QualificationRepositoryBuilder().Build(dbContextWrapper);

        var result = await repository.HasAny();
        result.Should().BeFalse();
    }

    [Fact]
    public async Task HasAny_Returns_True_When_Rows_Exist()
    {
        var dbConnection = Substitute.For<IDbConnection>();
        var dbContextWrapper = Substitute.For<IDbContextWrapper>();
        dbContextWrapper
            .CreateConnection()
            .Returns(dbConnection);
        dbContextWrapper
            .ExecuteScalarAsync<int>(dbConnection,
                Arg.Is<string>(s => s.Contains("dbo.Qualification")))
            .Returns(1);

        var repository = new QualificationRepositoryBuilder().Build(dbContextWrapper);

        var result = await repository.HasAny();
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Save_Calls_Database_As_Expected()
    {
        var qualifications = new QualificationBuilder()
            .BuildList()
            .ToList();

        var changeResult = new DataBaseChangeResultBuilder()
            .WithInserts(3)
            .WithUpdates(2)
            .WithDeletes(1)
            .Build();
            
        var receivedSqlArgs = new List<string>();

        var dbConnection = Substitute.For<IDbConnection>();
        var transaction = Substitute.For<IDbTransaction>();

        var dbContextWrapper = Substitute.For<IDbContextWrapper>();
        dbContextWrapper
            .CreateConnection()
            .Returns(dbConnection);
        dbContextWrapper
            .BeginTransaction(dbConnection)
            .Returns(transaction);

        dbContextWrapper
            .QueryAsync<(string Change, int ChangeCount)>(dbConnection,
                "UpdateQualifications",
                Arg.Any<object>(),
                Arg.Any<IDbTransaction>(),
                commandType: CommandType.StoredProcedure
            )
            .Returns(changeResult)
            .AndDoes(x =>
            {
                var arg = x.ArgAt<string>(1);
                receivedSqlArgs.Add(arg);
            });

        var pollyPolicy = PollyPolicyBuilder.BuildPolicy();
        var pollyPolicyRegistry = PollyPolicyBuilder.BuildPolicyRegistry(pollyPolicy);

        var logger = Substitute.For<ILogger<QualificationRepository>>();

        var repository = new QualificationRepositoryBuilder()
            .Build(dbContextWrapper, pollyPolicyRegistry, logger);

        await repository.Save(qualifications);

        await dbContextWrapper
            .Received(1)
            .QueryAsync<(string Change, int ChangeCount)>(
                dbConnection,
                Arg.Any<string>(),
                Arg.Is<object>(o => o != null),
                Arg.Is<IDbTransaction>(t => t != null),
                commandType: CommandType.StoredProcedure
            );

        receivedSqlArgs.Should().Contain("UpdateQualifications");

        dbContextWrapper
            .Received(1)
            .BeginTransaction(dbConnection);

        transaction
            .Received(1)
            .Commit();
    }

    [Fact]
    public async Task Save_Calls_Retry_Policy()
    {
        var pollyPolicy = PollyPolicyBuilder.BuildPolicy();
        var pollyPolicyRegistry = PollyPolicyBuilder.BuildPolicyRegistry(pollyPolicy);

        var logger = Substitute.For<ILogger<QualificationRepository>>();

        var repository = new QualificationRepositoryBuilder()
            .Build(policyRegistry: pollyPolicyRegistry, 
                logger: logger);

        await repository.Save(new List<Qualification>());
        
        await pollyPolicy.Received(1).ExecuteAsync(
            Arg.Any<Func<Context, Task>>(),
            Arg.Is<Context>(ctx =>
                ctx.ContainsKey(PolicyContextItems.Logger) &&
                ctx[PolicyContextItems.Logger] == logger
            ));
    }
}