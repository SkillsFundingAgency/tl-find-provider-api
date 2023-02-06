using System.Data;
using Dapper;
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
                "GetSearchFilterSummary",
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

        var results = (await repository.GetSearchFilterSummaryList(ukPrn, includeAdditionalData))
            .ToList();

        results.Should().NotBeNullOrEmpty();
        results!.Count.Should().Be(1);
        results.First().Validate(searchFilterDtoList.First());
        results[0].Routes.First().Id.Should().Be(routeDtoList[0].RouteId);
        results[0].Routes.First().Name.Should().Be(routeDtoList[0].RouteName);
    }

    [Fact]
    public async Task GetSearchFilterSummaryList_Sets_Dynamic_Parameters()
    {
        const long ukPrn = 12345678;
        const bool includeAdditionalData = true;

        var (dbContextWrapper, _, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new SearchFilterRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.GetSearchFilterSummaryList(ukPrn, includeAdditionalData);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(2);
        templates.ContainsNameAndValue("ukPrn", ukPrn);
        templates.ContainsNameAndValue("includeAdditionalData", includeAdditionalData);
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
                "GetSearchFilterDetail",
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
        result.Routes.Should().NotBeNullOrEmpty();
        result.Routes.First().Validate(routeDtoList[0]);
    }

    [Fact]
    public async Task GetSearchFilter_Sets_Dynamic_Parameters()
    {
        const int locationId = 1;
        
        var (dbContextWrapper, _, dynamicParametersWrapper) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnectionWithDynamicParameters();
        
        var repository = new SearchFilterRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.GetSearchFilter(locationId);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(1);
        templates.ContainsNameAndValue("locationId", locationId);
    }

    [Fact]
    public async Task SaveSearchFilter_Calls_Database()
    {
        var searchFilter = new SearchFilterBuilder()
            .Build();

        var (dbContextWrapper, dbConnection, dynamicParametersWrapper) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new SearchFilterRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.Save(searchFilter);

        await dbContextWrapper
            .Received(1)
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s => s == "CreateOrUpdateSearchFilter"),
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters),
                commandType: CommandType.StoredProcedure);
    }

    [Fact]
    public async Task SaveSearchFilter_Sets_Dynamic_Parameters()
    {
        var searchFilter = new SearchFilterBuilder()
            .Build();

        var (dbContextWrapper, _, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new SearchFilterRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.Save(searchFilter);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(3);
        templates.ContainsNameAndValue("locationId", searchFilter.LocationId);
        templates.ContainsNameAndValue("searchRadius", searchFilter.SearchRadius);
        var routeIds = templates.GetParameter<SqlMapper.ICustomQueryParameter>("routeIds");
        routeIds.Should().NotBeNull();
    }
}