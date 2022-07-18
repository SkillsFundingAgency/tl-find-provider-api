using System.Data;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Polly;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Models;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Policies;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Repositories;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Data;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Models;

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

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<Qualification>(dbConnection, Arg.Any<string>())
            .Returns(qualifications);

        const string expectedSql =
            "SELECT Id, Name FROM dbo.Qualification WHERE IsDeleted = 0 ORDER BY Name";

        var repository = new QualificationRepositoryBuilder().Build(dbContextWrapper);

        var results = (await repository.GetAll()).ToList();
        results.Should().BeEquivalentTo(qualifications);

        await dbContextWrapper
            .Received(1)
            .QueryAsync<Qualification>(dbConnection,
                Arg.Is<string>(sql => sql == expectedSql));
    }

    [Fact]
    public async Task HasAny_Returns_False_When_Zero_Rows_Exist()
    {
        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

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
        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

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
        
        var dapperParameterWrapper = new SubstituteDynamicParameterWrapper();

        var (dbContextWrapper, dbConnection, transaction) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnectionWithTransaction();

        dbContextWrapper
            .QueryAsync<(string Change, int ChangeCount)>(dbConnection,
                "UpdateQualifications",
                Arg.Any<object>(),//Is<object>(p => p == dynamicParameters),
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
            .Build(
                dbContextWrapper,
                dapperParameterWrapper.DapperParameterFactory,
                pollyPolicyRegistry, 
                logger);

        await repository.Save(qualifications);
        
        await dbContextWrapper
            .Received(1)
            .QueryAsync<(string Change, int ChangeCount)>(
                dbConnection,
                Arg.Any<string>(),
                Arg.Is<object>(o => o == dapperParameterWrapper.DynamicParameters),
                Arg.Is<IDbTransaction>(t => t == transaction),
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