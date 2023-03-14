using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;

public class ProviderRepositoryBuilder
{
    public IProviderRepository Build(
        IDbContextWrapper dbContextWrapper = null,
        IDynamicParametersWrapper dynamicParametersWrapper = null,
        IDateTimeProvider dateTimeProvider = null,
        IReadOnlyPolicyRegistry<string> policyRegistry = null,
        ILogger<ProviderRepository> logger = null)
    {
        dbContextWrapper ??= Substitute.For<IDbContextWrapper>();
        dateTimeProvider ??= Substitute.For<IDateTimeProvider>();
        dynamicParametersWrapper ??= Substitute.For<IDynamicParametersWrapper>();
        policyRegistry ??= Substitute.For<IReadOnlyPolicyRegistry<string>>();

        logger ??= Substitute.For<ILogger<ProviderRepository>>();
        
        return new ProviderRepository(
            dbContextWrapper,
            dynamicParametersWrapper,
            dateTimeProvider,
            policyRegistry, 
            logger);
    }

    public async Task<IProviderRepository> BuildRepositoryWithDataToSearchProviders(int totalLocationsCountToReturn)
    {
        var builder = new ProviderSearchResultBuilder();
        var providersPart = builder
            .BuildProvidersPartOfListWithSingleItem()
            .Take(1)
            .ToList();
        var deliveryYearsPart = builder
            .BuildDeliveryYearsPartOfListWithSingleItem()
            .Take(1)
            .ToList();
        var routesPart = builder
            .BuildRoutesPartOfListWithSingleItem()
            .Take(1)
            .ToList();
        var qualificationsPart = builder
            .BuildQualificationsPartOfListWithSingleItem()
            .Take(1)
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        var callIndex = 0;

        await dbContextWrapper
            .QueryAsync(dbConnection,
                "SearchProviders",
                Arg.Do<Func<ProviderSearchResult, DeliveryYearSearchResult, RouteDto, QualificationDto, ProviderSearchResult>>(
                    x =>
                    {
                        var p = providersPart[callIndex];
                        var d = deliveryYearsPart[callIndex];
                        var r = routesPart[callIndex];
                        var q = qualificationsPart[callIndex];
                        x.Invoke(p, d, r, q);
                        callIndex++;
                    }),
                Arg.Any<object>(),
                splitOn: Arg.Any<string>(),
                commandType: CommandType.StoredProcedure
            );

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(DateTime.Parse("2021-09-01"));

        var dynamicParametersWrapper = Substitute.For<IDynamicParametersWrapper>();
        var parameters = new DynamicParameters();
        parameters.Add("totalLocationsCount", totalLocationsCountToReturn, DbType.Int32, ParameterDirection.Output);
        dynamicParametersWrapper.DynamicParameters.Returns(parameters);

        return Build(
            dbContextWrapper,
            dynamicParametersWrapper: dynamicParametersWrapper,
            dateTimeProvider: dateTimeProvider);
    }

    public async Task<IProviderRepository> BuildRepositoryWithDataToGetAllProviders()
    {
        var builder = new ProviderDetailBuilder();
        var providersPart = builder
            .BuildProvidersPartOfListWithSingleItem()
            .Take(1)
            .ToList();
        var locationsPart = builder
            .BuildLocationsPartOfListWithSingleItem()
            .Take(1)
            .ToList();
        var deliveryYearsPart = builder
            .BuildDeliveryYearsPartOfListWithSingleItem()
            .Take(1)
            .ToList();
        var routesPart = builder
            .BuildRoutesPartOfListWithSingleItem()
            .Take(1)
            .ToList();
        var qualificationsPart = builder
            .BuildQualificationsPartOfListWithSingleItem()
            .Take(1)
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        var callIndex = 0;

        await dbContextWrapper
            .QueryAsync(dbConnection,
                "GetAllProviders",
                Arg.Do<Func<ProviderDetailDto, LocationDetailDto, DeliveryYearDetail, RouteDetail, QualificationDetail, ProviderDetail>>(
                    x =>
                    {
                        var p = providersPart[callIndex];
                        var l = locationsPart[callIndex];
                        var d = deliveryYearsPart[callIndex];
                        var r = routesPart[callIndex];
                        var q = qualificationsPart[callIndex];
                        x.Invoke(p, l, d, r, q);
                        callIndex++;
                    }),
                Arg.Any<object>(),
                splitOn: Arg.Any<string>(),
                commandType: CommandType.StoredProcedure
            );

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(DateTime.Parse("2021-09-01"));

        return Build(
            dbContextWrapper,
            dateTimeProvider: dateTimeProvider);
    }

    public IProviderRepository BuildRepositoryWithDataToGetAllFlattenedProviders()
    {
        var builder = new ProviderDetailFlatBuilder();
        var providerList = builder
            .BuildList()
            .Take(1)
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<ProviderDetailFlat>(dbConnection,
                "GetAllProviderDetails",
                commandType: CommandType.StoredProcedure)
            .Returns(providerList);

        return Build(dbContextWrapper);
    }
}