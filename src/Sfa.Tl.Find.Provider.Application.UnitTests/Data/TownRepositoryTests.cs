using System.Data;
using Microsoft.Extensions.Logging;
using Polly;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Policies;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;
using Sfa.Tl.Find.Provider.Application.UnitTests.TestHelpers.Data;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Data;

public class TownRepositoryTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(TownRepository)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task HasAny_Returns_False_When_Zero_Rows_Exist()
    {
        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .ExecuteScalarAsync<int>(dbConnection,
                Arg.Is<string>(s => s.Contains("dbo.Town")))
            .Returns(0);

        var repository = new TownRepositoryBuilder().Build(dbContextWrapper);

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
                Arg.Is<string>(s => s.Contains("dbo.Town")))
            .Returns(1);

        var repository = new TownRepositoryBuilder().Build(dbContextWrapper);

        var result = await repository.HasAny();
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Save_Calls_Database_As_Expected()
    {
        var towns = new TownBuilder()
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
                "UpdateTowns",
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

        var (_, pollyPolicyRegistry) = PollyPolicyBuilder
            .BuildDapperPolicyAndRegistry();

        var logger = Substitute.For<ILogger<TownRepository>>();

        var repository = new TownRepositoryBuilder()
            .Build(
                dbContextWrapper,
                dapperParameterWrapper.DapperParameterFactory,
                pollyPolicyRegistry, 
                logger);

        await repository.Save(towns);

        await dbContextWrapper
            .Received(1)
            .QueryAsync<(string Change, int ChangeCount)>(
                dbConnection,
                Arg.Any<string>(),
                Arg.Is<object>(o => o == dapperParameterWrapper.DynamicParameters),
                Arg.Is<IDbTransaction>(t => t == transaction),
                commandType: CommandType.StoredProcedure
            );

        receivedSqlArgs.Should().Contain("UpdateTowns");

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
        var (pollyPolicy, pollyPolicyRegistry) = PollyPolicyBuilder
            .BuildPolicyAndRegistry();

        var logger = Substitute.For<ILogger<TownRepository>>();

        var repository = new TownRepositoryBuilder()
            .Build(policyRegistry: pollyPolicyRegistry, 
                logger: logger);

        await repository.Save(new List<Town>());
        
        await pollyPolicy.Received(1).ExecuteAsync(
            Arg.Any<Func<Context, Task>>(),
            Arg.Is<Context>(ctx =>
                ctx.ContainsKey(PolicyContextItems.Logger) &&
                ctx[PolicyContextItems.Logger] == logger
            ));
    }

    [Fact]
    public async Task Search_Returns_Expected_Results()
    {
        const string searchTerm = "Coventry";
        const int maxResults = 10;

        var towns = new TownBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<Town>(dbConnection, Arg.Any<string>(), Arg.Any<object>())
            .Returns(towns);

        var repository = new TownRepositoryBuilder().Build(dbContextWrapper);

        var results = (await repository.Search(searchTerm, maxResults)).ToList();
        results.Should().BeEquivalentTo(towns);
        await dbContextWrapper
            .Received(1)
            .QueryAsync<Town>(dbConnection,
                Arg.Is<string>(sql => 
                    sql.Contains("FROM dbo.[TownSearchView]")),
                Arg.Any<object>());
    }
}