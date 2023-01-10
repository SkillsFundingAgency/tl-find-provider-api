using System.Data;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Data;

public class SearchFilterRepositoryTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(SearchFilterRepository)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task GetSearchFilters_Returns_Expected_Results()
    {
        const long ukPrn = 12345678;
        const bool includeAdditionalData = true;

        var searchFilters = new SearchFilterBuilder()
            .BuildList()
            .ToList();
        var searchFilterDtoList = new SearchFilterDtoBuilder()
            .BuildList()
            .ToList();
        var routeDtoList = new RouteDtoBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<SearchFilter>(dbConnection, Arg.Any<string>(), Arg.Any<object>())
            .Returns(searchFilters);

        var callIndex = 0;

        await dbContextWrapper
            .QueryAsync(dbConnection,
                "GetSearchFilters",
                Arg.Do<Func<SearchFilterDto, RouteDto, SearchFilter>>(
                    x =>
                    {
                        var e = searchFilterDtoList[callIndex];
                        var r = routeDtoList[callIndex];
                        x.Invoke(e, r);

                        callIndex++;
                    }),
                Arg.Any<object>(),
                splitOn: Arg.Any<string>(),
                commandType: CommandType.StoredProcedure
            );

        var repository = new SearchFilterRepositoryBuilder().Build(dbContextWrapper);

        var results = (await repository.GetSearchFilters(ukPrn, includeAdditionalData))
            .ToList();

        results.Should().NotBeNullOrEmpty();
        results!.Count.Should().Be(1);
        results.First().Validate(searchFilterDtoList.First());
        results[0].Routes.First().Id.Should().Be(routeDtoList[0].RouteId);
        results[0].Routes.First().Name.Should().Be(routeDtoList[0].RouteName);
    }

    [Fact]
    public async Task GetSearchFilter_Returns_Expected_Results()
    {
        const int id = 1;

        var searchFilters = new SearchFilterBuilder()
            .BuildList()
            .ToList();
        var searchFilterDtoList = new SearchFilterDtoBuilder()
            .BuildList()
            .Take(1)
            .ToList();
        var routeDtoList = new RouteDtoBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<SearchFilter>(dbConnection, Arg.Any<string>(), Arg.Any<object>())
            .Returns(searchFilters);

        var callIndex = 0;

        await dbContextWrapper
            .QueryAsync(dbConnection,
                "GetSearchFilter",
                Arg.Do<Func<SearchFilterDto, RouteDto, SearchFilter>>(
                    x =>
                    {
                        var e = searchFilterDtoList[callIndex];
                        var r = routeDtoList[callIndex];
                        x.Invoke(e, r);

                        callIndex++;
                    }),
                Arg.Any<object>(),
                splitOn: Arg.Any<string>(),
                commandType: CommandType.StoredProcedure
            );

        var repository = new SearchFilterRepositoryBuilder().Build(dbContextWrapper);

        var result = await repository.GetSearchFilter(id);

        result.Validate(searchFilterDtoList.First());
        result.Routes.First().Id.Should().Be(routeDtoList[0].RouteId);
        result.Routes.First().Name.Should().Be(routeDtoList[0].RouteName);
    }
}