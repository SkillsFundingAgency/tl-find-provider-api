using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using Polly;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Policies;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Data;

public class EmployerInterestRepositoryTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(EmployerInterestRepository)
            .ShouldNotAcceptNullConstructorArguments();
    }
    
    [Fact]
    public async Task Create_Calls_Retry_Policy()
    {
        var employerInterest = new EmployerInterestBuilder()
            .Build();

        var pollyPolicy = PollyPolicyBuilder.BuildPolicy<(int, Guid)>();
        var pollyPolicyRegistry = PollyPolicyBuilder.BuildPolicyRegistry(pollyPolicy);

        var logger = Substitute.For<ILogger<EmployerInterestRepository>>();

        var repository = new EmployerInterestRepositoryBuilder()
            .Build(
                policyRegistry: pollyPolicyRegistry,
                logger: logger);

        await repository.Create(employerInterest);

        await pollyPolicy.Received(1).ExecuteAsync(
            Arg.Any<Func<Context, Task<(int, Guid)>>> (),
            Arg.Is<Context>(ctx =>
                ctx.ContainsKey(PolicyContextItems.Logger) &&
                ctx[PolicyContextItems.Logger] == logger
            ));
    }

    [Fact]
    public async Task Create_Calls_Database_As_Expected()
    {
        var employerInterest = new EmployerInterestBuilder()
            .Build();

        var (dbContextWrapper, dbConnection, transaction) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnectionWithTransaction();

        var pollyPolicy = PollyPolicyBuilder.BuildPolicy<(int, Guid)>();
        var pollyPolicyRegistry = PollyPolicyBuilder.BuildPolicyRegistry(pollyPolicy);

        var repository = new EmployerInterestRepositoryBuilder()
            .Build(dbContextWrapper,
                policyRegistry: pollyPolicyRegistry);

        await repository.Create(employerInterest);

        dbContextWrapper
            .Received(1)
            .BeginTransaction(dbConnection);

        await dbContextWrapper
            .Received(1)
            .ExecuteAsync(dbConnection,
                "CreateEmployerInterest",
                Arg.Any<object>(),
                Arg.Is<IDbTransaction>(t => t != null),
                commandType: CommandType.StoredProcedure);

        transaction
            .Received(1)
            .Commit();
    }

    [Fact]
    public async Task Create_Returns_Expected_Result()
    {
        var uniqueId = new Guid();
        var guidService = Substitute.For<IGuidService>();
        guidService
            .NewGuid()
            .Returns(uniqueId);

        var employerInterest = new EmployerInterestBuilder()
            .Build();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s =>
                    s == "CreateEmployerInterest"),
                Arg.Any<object>(),
                Arg.Is<IDbTransaction>(t => t != null),
                commandType: CommandType.StoredProcedure)
            .Returns(1);

        var pollyPolicy = PollyPolicyBuilder.BuildPolicy<(int, Guid) >();
        var pollyPolicyRegistry = PollyPolicyBuilder.BuildPolicyRegistry(pollyPolicy);
        
        var repository = new EmployerInterestRepositoryBuilder()
            .Build(dbContextWrapper,
                policyRegistry: pollyPolicyRegistry,
                guidService: guidService);

        var result = await repository.Create(employerInterest);

        result.Count.Should().Be(1);
        result.UniqueId.Should().Be(uniqueId);
    }
    
    [Fact]
    public async Task Delete_Calls_Retry_Policy()
    {
        var uniqueId = Guid.Parse("5FBDFA5D-3987-4A3D-B4A2-DBAF545455CB");

        var pollyPolicy = PollyPolicyBuilder.BuildPolicy();
        var pollyPolicyRegistry = PollyPolicyBuilder.BuildPolicyRegistry(pollyPolicy);

        var logger = Substitute.For<ILogger<EmployerInterestRepository>>();

        var repository = new EmployerInterestRepositoryBuilder()
            .Build(
                policyRegistry: pollyPolicyRegistry,
                logger: logger);

        await repository.Delete(uniqueId);

        await pollyPolicy.Received(1).ExecuteAsync(
            Arg.Any<Func<Context, Task<int>>>(),
            Arg.Is<Context>(ctx =>
                ctx.ContainsKey(PolicyContextItems.Logger) &&
                ctx[PolicyContextItems.Logger] == logger
            ));
    }

    [Fact]
    public async Task Delete_Calls_Database_As_Expected()
    {
        const int id = 101;
        var uniqueId = Guid.Parse("5FBDFA5D-3987-4A3D-B4A2-DBAF545455CB");

        var (dbContextWrapper, dbConnection, transaction) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnectionWithTransaction();

        dbContextWrapper
            .ExecuteScalarAsync<int?>(dbConnection,
                Arg.Is<string>(s =>
                    s.Contains("SELECT Id")),
                Arg.Any<object>(),
                Arg.Is<IDbTransaction>(t => t != null))
            .Returns(id);

        var pollyPolicy = PollyPolicyBuilder.BuildPolicy<int>();
        var pollyPolicyRegistry = PollyPolicyBuilder.BuildPolicyRegistry(pollyPolicy);

        var repository = new EmployerInterestRepositoryBuilder()
            .Build(dbContextWrapper,
                policyRegistry: pollyPolicyRegistry);

        await repository.Delete(uniqueId);

        dbContextWrapper
            .Received(1)
            .BeginTransaction(dbConnection);

        await dbContextWrapper
            .Received(1)
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s =>
                    s.Contains("DeleteEmployerInterest")),
                Arg.Any<object>(),
                Arg.Is<IDbTransaction>(t => t != null),
                commandType: CommandType.StoredProcedure);

        transaction
            .Received(1)
            .Commit();
    }

    [Fact]
    public async Task Delete_Returns_Expected_Result()
    {
        const int count = 1;
        const int id = 101;
        var uniqueId = Guid.Parse("5FBDFA5D-3987-4A3D-B4A2-DBAF545455CB");

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .ExecuteScalarAsync<int?>(dbConnection,
                Arg.Is<string>(s =>
                    s.Contains("SELECT Id")),
                Arg.Any<object>(),
                Arg.Is<IDbTransaction>(t => t != null))
            .Returns(id);

        dbContextWrapper
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s =>
                    s.Contains("DeleteEmployerInterest")),
                Arg.Any<object>(),
                Arg.Is<IDbTransaction>(t => t != null),
                commandType: CommandType.StoredProcedure)
        .Returns(count);

        var pollyPolicy = PollyPolicyBuilder.BuildPolicy<int>();
        var pollyPolicyRegistry = PollyPolicyBuilder.BuildPolicyRegistry(pollyPolicy);
        
        var logger = Substitute.For<ILogger<EmployerInterestRepository>>();

        var repository = new EmployerInterestRepositoryBuilder().Build(
            dbContextWrapper,
            policyRegistry: pollyPolicyRegistry,
            logger: logger);

        var result = await repository.Delete(uniqueId);

        result.Should().Be(count);
    }

    [Fact]
    public async Task DeleteBefore_Returns_Expected_Result()
    {
        const int count = 10;
        var date = DateTime.Parse("2022-09-13");

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<int>(dbConnection,
                Arg.Is<string>(s =>
                    s.Contains("SELECT Id") &&
                    s.Contains("FROM [dbo].[EmployerInterest]") &&
                    s.Contains("[CreatedOn] < @date")),
                Arg.Any<object>(),
                Arg.Is<IDbTransaction>(t => t != null))
            .Returns(Enumerable.Range(1, 10));

        dbContextWrapper
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s =>
                    s.Contains("DeleteEmployerInterest")),
                Arg.Any<object>(),
                Arg.Is<IDbTransaction>(t => t != null),
                commandType: CommandType.StoredProcedure)
            .Returns(count);

        var repository = new EmployerInterestRepositoryBuilder().Build(dbContextWrapper);

        var result = await repository.DeleteBefore(date);

        result.Should().Be(count);
    }

    [Fact]
    public async Task Get_Returns_Expected_Item()
    {
        var employerInterest = new EmployerInterestBuilder()
            .Build();
        var id = employerInterest.Id;

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<EmployerInterest>(dbConnection,
                Arg.Any<string>(),
                Arg.Any<DynamicParameters>())
            .Returns(new List<EmployerInterest> { employerInterest });

        var repository = new EmployerInterestRepositoryBuilder().Build(dbContextWrapper);

        var result = await repository.Get(id);
        result.Should().BeEquivalentTo(employerInterest);

        await dbContextWrapper
            .Received(1)
            .QueryAsync<EmployerInterest>(dbConnection,
                Arg.Is<string>(sql => sql.Contains("FROM dbo.EmployerInterest")));
    }

    [Fact]
    public async Task GetAll_Returns_Expected_List()
    {
        var employerInterests = new EmployerInterestBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<EmployerInterest>(dbConnection,
                Arg.Any<string>())
            .Returns(employerInterests);

        var repository = new EmployerInterestRepositoryBuilder().Build(dbContextWrapper);

        var results = (await repository.GetAll()).ToList();
        results.Should().BeEquivalentTo(employerInterests);

        await dbContextWrapper
            .Received(1)
            .QueryAsync<EmployerInterest>(dbConnection,
                Arg.Is<string>(sql => sql.Contains("FROM dbo.EmployerInterest")));
    }

    [Fact]
    public async Task GetSummaryList_Returns_Expected_List()
    {
        var employerInterestSummaryList = new EmployerInterestSummaryBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<EmployerInterestSummary>(dbConnection,
                "GetAllEmployerInterest",
                commandType: CommandType.StoredProcedure)
            .Returns(employerInterestSummaryList);

        var repository = new EmployerInterestRepositoryBuilder().Build(dbContextWrapper);

        var results = (await repository.GetSummaryList()).ToList();
        results.Should().BeEquivalentTo(employerInterestSummaryList);
    }
}