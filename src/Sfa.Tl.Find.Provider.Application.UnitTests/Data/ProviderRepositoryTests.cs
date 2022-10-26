using System.Data;
using Microsoft.Extensions.Logging;
using Polly;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Policies;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Data;

public class ProviderRepositoryTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(ProviderRepository)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task GetAll_Returns_Expected_List_For_Single_Result_Row()
    {
        var expectedResult = new ProviderDetailBuilder()
            .BuildListWithSingleItem();

        var repository = await new ProviderRepositoryBuilder()
            .BuildRepositoryWithDataToGetAllProviders();

        var results = (await repository
            .GetAll())
            ?.ToList();

        results.Should().NotBeNull();
        results!.Count.Should().Be(1);

        results.First().Validate(expectedResult.First());
    }

    [Fact]
    public async Task GetAllFlattened_Returns_Expected_List_For_Single_Result_Row()
    {
        var expectedResult = new ProviderDetailFlatBuilder()
            .BuildListWithSingleItem();

        var repository = new ProviderRepositoryBuilder()
            .BuildRepositoryWithDataToGetAllFlattenedProviders();

        var results = (await repository
                .GetAllFlattened())
            ?.ToList();

        results.Should().NotBeNull();
        results!.Count.Should().Be(1);

        results.First().Validate(expectedResult.First());
    }

    [Fact]
    public async Task GetLocationPostcodes_Returns_Expected_List_For_Single_Result_Row()
    {
        const long ukPrn = 12345678;
        const bool includeAdditionalData = true;

        var locationPostcodes = new LocationPostcodeBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<LocationPostcode>(dbConnection,
                "GetProviderLocations",
                Arg.Any<object>(),
                commandType: CommandType.StoredProcedure)
            .Returns(locationPostcodes);

        var repository = new ProviderRepositoryBuilder()
            .Build(dbContextWrapper);

        var results = (await repository
                .GetLocationPostcodes(ukPrn, includeAdditionalData))
            ?.ToList();

        results.Should().NotBeNull();
        results!.Count.Should().Be(locationPostcodes.Count);

        results.Should().BeEquivalentTo(locationPostcodes);
        results.First().Validate(locationPostcodes.First());
    }

    [Fact]
    public async Task HasAny_Returns_False_When_Zero_Rows_Exist()
    {
        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .ExecuteScalarAsync<int>(dbConnection,
                Arg.Is<string>(s => s.Contains("dbo.Provider")))
            .Returns(0);

        var repository = new ProviderRepositoryBuilder().Build(dbContextWrapper);

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
                Arg.Any<string>(),
                Arg.Any<object>())
            .Returns(1);

        var repository = new ProviderRepositoryBuilder().Build(dbContextWrapper);

        var result = await repository.HasAny();
        result.Should().BeTrue();

        await dbContextWrapper
            .Received(1)
            .ExecuteScalarAsync<int>(dbConnection,
                Arg.Is<string>(s => s.Contains("dbo.Provider")),
                Arg.Is<object>(o => o.GetIsAdditionalDataValueFromAnonymousType() == 0));
    }

    [Fact]
    public async Task HasAny_Returns_True_When_Rows_Exist_For_Additional_Data()
    {
        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .ExecuteScalarAsync<int>(dbConnection,
                Arg.Any<string>(),
                Arg.Any<object>())
            .Returns(1);

        var repository = new ProviderRepositoryBuilder().Build(dbContextWrapper);

        var result = await repository.HasAny(true);
        result.Should().BeTrue();

        await dbContextWrapper
            .Received(1)
            .ExecuteScalarAsync<int>(dbConnection,
                Arg.Is<string>(s => s.Contains("dbo.Provider")),
                Arg.Is<object>(o => o.GetIsAdditionalDataValueFromAnonymousType() == 1));
    }

    [Fact]
    public async Task Save_Calls_Database_As_Expected()
    {
        var providers = new ProviderBuilder()
            .BuildList()
            .ToList();

        var providersChangeResult = new DataBaseChangeResultBuilder()
            .WithInserts(10)
            .WithUpdates(5)
            .WithDeletes(2)
            .Build();
        var locationsChangeResult = new DataBaseChangeResultBuilder()
            .WithInserts(50)
            .WithUpdates(30)
            .WithDeletes(10)
            .Build();
        var locationQualificationsChangeResult = new DataBaseChangeResultBuilder()
            .WithInserts(100)
            .WithDeletes(20)
            .Build();

        var receivedSqlArgs = new List<string>();

        var (dbContextWrapper, dbConnection, transaction) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnectionWithTransaction();

        dbContextWrapper
            .QueryAsync<(string Change, int ChangeCount)>(dbConnection,
                "UpdateProviders",
                Arg.Any<object>(),
                Arg.Any<IDbTransaction>(),
                commandType: CommandType.StoredProcedure
            )
            .Returns(providersChangeResult)
            .AndDoes(x =>
            {
                var arg = x.ArgAt<string>(1);
                receivedSqlArgs.Add(arg);
            });
        dbContextWrapper
            .QueryAsync<(string Change, int ChangeCount)>(dbConnection,
                "UpdateLocations",
                Arg.Any<object>(),
                Arg.Any<IDbTransaction>(),
                commandType: CommandType.StoredProcedure
            )
            .Returns(locationsChangeResult)
            .AndDoes(x =>
            {
                var arg = x.ArgAt<string>(1);
                receivedSqlArgs.Add(arg);
            });
        dbContextWrapper
            .QueryAsync<(string Change, int ChangeCount)>(dbConnection,
                "UpdateLocationQualifications",
                Arg.Any<object>(),
                Arg.Any<IDbTransaction>(),
                commandType: CommandType.StoredProcedure
            )
            .Returns(locationQualificationsChangeResult)
            .AndDoes(x =>
            {
                var arg = x.ArgAt<string>(1);
                receivedSqlArgs.Add(arg);
            });

        var (_, pollyPolicyRegistry) = PollyPolicyBuilder
            .BuildDapperPolicyAndRegistry();

        var logger = Substitute.For<ILogger<ProviderRepository>>();

        var repository = new ProviderRepositoryBuilder()
            .Build(
                dbContextWrapper,
                policyRegistry: pollyPolicyRegistry,
                logger: logger);

        await repository.Save(providers);

        await dbContextWrapper
            .Received(3)
            .QueryAsync<(string Change, int ChangeCount)>(
                dbConnection,
                Arg.Any<string>(),
                Arg.Any<object>(),
                Arg.Is<IDbTransaction>(t => t == transaction),
                commandType: CommandType.StoredProcedure
            );

        receivedSqlArgs.Should().Contain("UpdateProviders");
        receivedSqlArgs.Should().Contain("UpdateLocations");
        receivedSqlArgs.Should().Contain("UpdateLocationQualifications");

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
            .BuildDapperPolicyAndRegistry();

        var logger = Substitute.For<ILogger<ProviderRepository>>();

        var repository = new ProviderRepositoryBuilder()
            .Build(
                policyRegistry: pollyPolicyRegistry,
                logger: logger);

        await repository.Save(new List<Application.Models.Provider>());

        await pollyPolicy.Received(1).ExecuteAsync(
            Arg.Any<Func<Context, Task>>(),
            Arg.Is<Context>(ctx =>
                ctx.ContainsKey(PolicyContextItems.Logger) &&
                ctx[PolicyContextItems.Logger] == logger
            ));
    }

    [Fact]
    public async Task Search_Returns_Expected_List_For_Single_Result_Row()
    {
        var fromGeoLocation = GeoLocationBuilder.BuildValidPostcodeLocation();
        const int totalLocationsCount = 1234;

        var expectedResult = new ProviderSearchResultBuilder()
            .BuildSingleSearchResultWithSearchOrigin(fromGeoLocation);

        var repository = await new ProviderRepositoryBuilder()
            .BuildRepositoryWithDataToSearchProviders(totalLocationsCount);

        var searchResults = await repository.Search(fromGeoLocation, null, null, 0, 5, false);

        searchResults.Should().NotBeNull();
        var searchResultsList = searchResults.SearchResults?.ToList();
        searchResultsList.Should().NotBeNull();
        searchResultsList!.Count.Should().Be(1);
        searchResults.TotalResultsCount.Should().Be(totalLocationsCount);

        searchResultsList.First().Validate(expectedResult);
    }

    [Fact]
    public async Task Search_Merges_Additional_Data()
    {
        var fromGeoLocation = GeoLocationBuilder.BuildValidPostcodeLocation();
        const int totalLocationsCount = 1234;

        var expectedResult = new ProviderSearchResultBuilder()
             .BuildSingleSearchResultWithSearchOrigin(fromGeoLocation);

        var repository = await new ProviderRepositoryBuilder()
            .BuildRepositoryWithDataToSearchProviders(totalLocationsCount);

        var searchResults = await repository
            .Search(
                fromGeoLocation,
                null,
                null,
                0,
                5,
                true);

        searchResults.Should().NotBeNull();
        var searchResultsList = searchResults.SearchResults?.ToList();
        searchResultsList.Should().NotBeNull();
        searchResultsList!.Count.Should().Be(1);
        searchResults.TotalResultsCount.Should().Be(totalLocationsCount);

        searchResultsList.First().Validate(expectedResult);
    }
}