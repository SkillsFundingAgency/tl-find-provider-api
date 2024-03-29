﻿using System.Data;
using Microsoft.Extensions.Logging;
using Polly;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Policies;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
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
        var employerInterest = new EmployerInterestBuilder().Build();
        var geoLocation = GeoLocationBuilder.BuildValidPostcodeLocation();
        var expiryDate = DateTime.Parse("2022-12-31 23:59:59.9999999");

        var pollyPolicy = PollyPolicyBuilder.BuildPolicy<(int, Guid)>();
        var pollyPolicyRegistry = PollyPolicyBuilder.BuildPolicyRegistry(pollyPolicy);

        var logger = Substitute.For<ILogger<EmployerInterestRepository>>();

        var repository = new EmployerInterestRepositoryBuilder()
            .Build(
                policyRegistry: pollyPolicyRegistry,
                logger: logger);

        await repository.Create(employerInterest, geoLocation, expiryDate);

        await pollyPolicy.Received(1).ExecuteAsync(
            Arg.Any<Func<Context, Task<(int, Guid)>>>(),
            Arg.Is<Context>(ctx =>
                ctx.ContainsKey(PolicyContextItems.Logger) &&
                ctx[PolicyContextItems.Logger] == logger
            ));
    }

    [Fact]
    public async Task Create_Calls_Database_As_Expected()
    {
        var employerInterest = new EmployerInterestBuilder().Build();
        var geoLocation = GeoLocationBuilder.BuildValidPostcodeLocation();
        var expiryDate = DateTime.Parse("2022-12-31 23:59:59.9999999");

        var (dbContextWrapper, dbConnection, transaction) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnectionWithTransaction();

        var pollyPolicy = PollyPolicyBuilder.BuildPolicy<(int, Guid)>();
        var pollyPolicyRegistry = PollyPolicyBuilder.BuildPolicyRegistry(pollyPolicy);

        var repository = new EmployerInterestRepositoryBuilder()
            .Build(dbContextWrapper,
                policyRegistry: pollyPolicyRegistry);

        await repository.Create(employerInterest, geoLocation, expiryDate);

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
        var guidProvider = Substitute.For<IGuidProvider>();
        guidProvider
            .NewGuid()
            .Returns(uniqueId);

        var employerInterest = new EmployerInterestBuilder().Build();
        var geoLocation = GeoLocationBuilder.BuildValidPostcodeLocation();
        var expiryDate = DateTime.Parse("2022-12-31 23:59:59.9999999");

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

        var pollyPolicy = PollyPolicyBuilder.BuildPolicy<(int, Guid)>();
        var pollyPolicyRegistry = PollyPolicyBuilder.BuildPolicyRegistry(pollyPolicy);

        var repository = new EmployerInterestRepositoryBuilder()
            .Build(dbContextWrapper,
                policyRegistry: pollyPolicyRegistry,
                guidProvider: guidProvider);

        var result = await repository.Create(employerInterest, geoLocation, expiryDate);

        result.Count.Should().Be(1);
        result.UniqueId.Should().Be(uniqueId);
    }

    [Fact]
    public async Task Delete_By_Id_Calls_Retry_Policy()
    {
        const int id = 101;

        var pollyPolicy = PollyPolicyBuilder.BuildPolicy();
        var pollyPolicyRegistry = PollyPolicyBuilder.BuildPolicyRegistry(pollyPolicy);

        var logger = Substitute.For<ILogger<EmployerInterestRepository>>();

        var repository = new EmployerInterestRepositoryBuilder()
            .Build(
                policyRegistry: pollyPolicyRegistry,
                logger: logger);

        await repository.Delete(id);

        await pollyPolicy.Received(1).ExecuteAsync(
            Arg.Any<Func<Context, Task<int>>>(),
            Arg.Is<Context>(ctx =>
                ctx.ContainsKey(PolicyContextItems.Logger) &&
                ctx[PolicyContextItems.Logger] == logger
            ));
    }

    [Fact]
    public async Task Delete_By_Id_Calls_Database_As_Expected()
    {
        const int id = 101;
        const int employerInterestsCount = 1;

        var (dbContextWrapper, dbConnection, transaction) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnectionWithTransaction();

        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder()
            .BuildWithOutputParameter("employerInterestsDeleted", employerInterestsCount);

        var pollyPolicy = PollyPolicyBuilder.BuildPolicy<int>();
        var pollyPolicyRegistry = PollyPolicyBuilder.BuildPolicyRegistry(pollyPolicy);

        var repository = new EmployerInterestRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper,
                policyRegistry: pollyPolicyRegistry);

        var results = await repository.Delete(id);

        results.Should().Be(employerInterestsCount);

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
    public async Task Delete_By_Id_Returns_Expected_Result()
    {
        const int id = 101;
        const int employerInterestsCount = 1;

        var dbContextWrapper = new DbContextWrapperBuilder()
            .BuildSubstitute();

        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder()
            .BuildWithOutputParameter("employerInterestsDeleted", employerInterestsCount);
        
        var pollyPolicy = PollyPolicyBuilder.BuildPolicy<int>();
        var pollyPolicyRegistry = PollyPolicyBuilder.BuildPolicyRegistry(pollyPolicy);

        var logger = Substitute.For<ILogger<EmployerInterestRepository>>();

        var repository = new EmployerInterestRepositoryBuilder().Build(
            dbContextWrapper,
            dynamicParametersWrapper,
            policyRegistry: pollyPolicyRegistry,
            logger: logger);

        var result = await repository.Delete(id);

        result.Should().Be(employerInterestsCount);
    }

    [Fact]
    public async Task Delete_By_UniqueId_Calls_Retry_Policy()
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
    public async Task Delete_By_UniqueId_Calls_Database_As_Expected()
    {
        const int id = 101;
        const int employerInterestsCount = 101;
        var uniqueId = Guid.Parse("5FBDFA5D-3987-4A3D-B4A2-DBAF545455CB");

        var (dbContextWrapper, dbConnection, transaction) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnectionWithTransaction();

        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder()
            .BuildWithOutputParameter("employerInterestsDeleted", employerInterestsCount);

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
                dynamicParametersWrapper,
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
    public async Task Delete_By_UniqueId_Returns_Expected_Result()
    {
        const int id = 101;
        const int employerInterestsCount = 1;
        var uniqueId = Guid.Parse("5FBDFA5D-3987-4A3D-B4A2-DBAF545455CB");

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder()
            .BuildWithOutputParameter("employerInterestsDeleted", employerInterestsCount);

        dbContextWrapper
            .ExecuteScalarAsync<int?>(dbConnection,
                Arg.Is<string>(s =>
                    s.Contains("SELECT Id")),
                Arg.Any<object>(),
                Arg.Is<IDbTransaction>(t => t != null))
            .Returns(id);

        var pollyPolicy = PollyPolicyBuilder.BuildPolicy<int>();
        var pollyPolicyRegistry = PollyPolicyBuilder.BuildPolicyRegistry(pollyPolicy);

        var logger = Substitute.For<ILogger<EmployerInterestRepository>>();

        var repository = new EmployerInterestRepositoryBuilder().Build(
            dbContextWrapper,
            dynamicParametersWrapper,
            policyRegistry: pollyPolicyRegistry,
            logger: logger);

        var result = await repository.Delete(uniqueId);

        result.Should().Be(employerInterestsCount);
    }

    [Fact]
    public async Task DeleteExpired_Returns_Expected_Result()
    {
        var date = DateTime.Parse("2022-09-13");

        var expiredEmployerInterest = new ExpiredEmployerInterestDtoBuilder()
            .BuildList()
            .ToList();
        var employerInterestsCount = expiredEmployerInterest.Count;

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder()
            .BuildWithOutputParameter("employerInterestsDeleted", employerInterestsCount);

        dbContextWrapper
            .QueryAsync<ExpiredEmployerInterestDto>(dbConnection,
                Arg.Is<string>(s =>
                    s.Contains("SELECT ei.Id, ei.UniqueId, ei.OrganisationName, eil.Postcode, ei.Email") &&
                    s.Contains("FROM [dbo].[EmployerInterest] ei") &&
                    s.Contains("LEFT JOIN [dbo].[EmployerInterestLocation] eil") &&
                    s.Contains("ON eil.[EmployerInterestId] = ei.[Id]") &&
                    s.Contains("[ExpiryDate] < @date")),
                Arg.Any<object>(),
                Arg.Is<IDbTransaction>(t => t != null))
            .Returns(expiredEmployerInterest);

        var repository = new EmployerInterestRepositoryBuilder()
            .Build(dbContextWrapper, 
                dynamicParametersWrapper);

        var results = (await repository
            .DeleteExpired(date))
            .ToList();

        results.Should().NotBeNullOrEmpty();
        results.Count().Should().Be(employerInterestsCount);
        results.Should().BeEquivalentTo(expiredEmployerInterest);
    }
    
    [Fact]
    public async Task GetDetail_Returns_Expected_Item()
    {
        var uniqueId = Guid.Parse("69e33e1f-2dc3-40bf-a1a7-52493025d3d1");

        var employerInterestDetailDto = new EmployerInterestDetailDtoBuilder()
            .WithUniqueId(uniqueId)
            .Build();
        var routeDtoList = new RouteDtoBuilder()
            .BuildList()
            .Where(r => r.RouteName is "Digital and IT")
            .ToList();

        var expectedEmployerInterestDetail = new EmployerInterestDetailBuilder()
            .WithUniqueId(uniqueId)
            .WithSkillAreas(routeDtoList
                .Select(r => r.RouteName)
                .ToList())
            .Build();

        var id = employerInterestDetailDto.Id;

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        var callIndex = 0;

        await dbContextWrapper
            .QueryAsync(dbConnection,
                "GetEmployerInterestDetail",
                Arg.Do<Func<EmployerInterestDetailDto, RouteDto, EmployerInterestDetail>>(
                    x =>
                    {
                        var e = employerInterestDetailDto;
                        var r = routeDtoList[callIndex];
                        x.Invoke(e, r);

                        callIndex++;
                    }),
                Arg.Any<object>(),
                splitOn: Arg.Any<string>(),
                commandType: CommandType.StoredProcedure
            );

        var repository = new EmployerInterestRepositoryBuilder().Build(dbContextWrapper);

        var result = await repository.GetDetail(id);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedEmployerInterestDetail);

        callIndex.Should().Be(1);
    }

    [Fact]
    public async Task GetExpiringInterest_Returns_Expected_Item()
    {
        const int expiryNotificationDays = 7;
        var uniqueId = Guid.Parse("69e33e1f-2dc3-40bf-a1a7-52493025d3d1");

        var routeIdList = new RouteDtoBuilder()
            .BuildList()
            .Where(r => r.RouteName is "Digital and IT")
            .Select(r => r.RouteId)
            .ToList(); 
        
        var employerInterest = new EmployerInterestBuilder()
            .WithUniqueId(uniqueId)
            .WithLatLong(0, 0)
            .WithIndustryId(9)
            .WithOtherIndustry("Testing")
            .WithSkillAreaIds(routeIdList)
            .Build();
        
        var expectedEmployerInterest = new EmployerInterestBuilder()
            .WithUniqueId(uniqueId)
            .WithLatLong(0, 0)
            .WithIndustryId(9)
            .WithOtherIndustry("Testing")
            .WithSkillAreaIds(routeIdList)
            .Build();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        var callIndex = 0;

        await dbContextWrapper
            .QueryAsync(dbConnection,
                "GetExpiringEmployerInterest",
                //Arg.Do<Func<EmployerInterest, RouteDto, EmployerInterest>>(
                Arg.Do<Func<EmployerInterest, int, EmployerInterest>>(
                    x =>
                    {
                        var e = employerInterest;
                        var r = routeIdList[callIndex];
                        x.Invoke(e, r);

                        callIndex++;
                    }),
                Arg.Any<object>(),
                splitOn: Arg.Any<string>(),
                commandType: CommandType.StoredProcedure
            );

        var repository = new EmployerInterestRepositoryBuilder().Build(dbContextWrapper);

        var result = (await repository
            .GetExpiringInterest(expiryNotificationDays))
            .ToList();

        result.Should().NotBeNullOrEmpty();
        result.First().Should().BeEquivalentTo(expectedEmployerInterest);

        callIndex.Should().Be(1);
    }

    [Fact]
    public async Task GetSummaryList_Returns_Expected_List()
    {
        var employerInterestSummaryDtoList = new EmployerInterestSummaryDtoBuilder()
            .BuildList()
            .ToList();
        var routeDtoList = new RouteDtoBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        var callIndex = 0;

        await dbContextWrapper
            .QueryAsync(dbConnection,
                "GetEmployerInterestSummary",
                Arg.Do<Func<EmployerInterestSummaryDto, RouteDto, EmployerInterestSummary>>(
                    x =>
                    {
                        var e = employerInterestSummaryDtoList[callIndex];
                        var r = routeDtoList[callIndex];
                        x.Invoke(e, r);

                        callIndex++;
                    }),
                Arg.Any<object>(),
                splitOn: Arg.Any<string>(),
                commandType: CommandType.StoredProcedure
            );

        var repository = new EmployerInterestRepositoryBuilder().Build(dbContextWrapper);

        var results = (await repository.GetSummaryList()).ToList();
        results.Should().NotBeNullOrEmpty();
        results.Count.Should().Be(1);
        results.First().Validate(employerInterestSummaryDtoList.First());
        results[0].SkillAreas[0].Should().Be(routeDtoList[0].RouteName);
    }

    [Fact]
    public async Task Search_By_Lat_Long_Returns_Expected_List()
    {
        const double latitude = 52.400997;
        const double longitude = -1.508122;
        const int searchRadius = 25;
        const int employerInterestsCount = 1000;

        var employerInterestSummaryDtoList = new EmployerInterestSummaryDtoBuilder()
            .BuildList()
            .ToList();
        var routeDtoList = new RouteDtoBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder()
            .BuildWithOutputParameter("totalEmployerInterestsCount", employerInterestsCount);

        var callIndex = 0;

        await dbContextWrapper
            .QueryAsync(dbConnection,
                "SearchEmployerInterest",
                Arg.Do<Func<EmployerInterestSummaryDto, RouteDto, EmployerInterestSummary>>(
                    x =>
                    {
                        var e = employerInterestSummaryDtoList[callIndex];
                        var r = routeDtoList[callIndex];
                        x.Invoke(e, r);

                        callIndex++;
                    }),
                Arg.Any<object>(),
                splitOn: Arg.Any<string>(),
                commandType: CommandType.StoredProcedure
            );

        var repository = new EmployerInterestRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper);

        var results = await repository.Search(latitude, longitude, searchRadius);

        var searchResults = results.SearchResults?.ToList();

        searchResults.Should().NotBeNullOrEmpty();
        searchResults!.Count.Should().Be(1);
        results.TotalResultsCount.Should().Be(employerInterestsCount);
        searchResults.First().Validate(employerInterestSummaryDtoList.First());
        searchResults[0].SkillAreas[0].Should().Be(routeDtoList[0].RouteName);
    }

    [Fact]
    public async Task Search_By_Location_Returns_Expected_List()
    {
        const int locationId = 1;
        const int defaultSearchRadius = 25;
        const int employerInterestsCount = 1000;
        const bool searchFiltersApplied = true;

        var employerInterestSummaryDtoList = new EmployerInterestSummaryDtoBuilder()
            .BuildList()
            .ToList();
        var routeDtoList = new RouteDtoBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder()
            .BuildWithOutputParameters(
                new List<(string, object, DbType)>
                {
                    new ("totalEmployerInterestsCount", employerInterestsCount, DbType.Int32),
                    new ("@searchFiltersApplied", searchFiltersApplied, DbType.Binary)
                });

        var callIndex = 0;

        await dbContextWrapper
            .QueryAsync(dbConnection,
                "SearchEmployerInterestByLocation",
                Arg.Do<Func<EmployerInterestSummaryDto, RouteDto, EmployerInterestSummary>>(
                    x =>
                    {
                        var e = employerInterestSummaryDtoList[callIndex];
                        var r = routeDtoList[callIndex];
                        x.Invoke(e, r);

                        callIndex++;
                    }),
                Arg.Any<object>(),
                splitOn: Arg.Any<string>(),
                commandType: CommandType.StoredProcedure
            );

        var repository = new EmployerInterestRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper);

        var results = await repository.Search(locationId, defaultSearchRadius);

        var searchResults = results.SearchResults?.ToList();

        searchResults.Should().NotBeNullOrEmpty();
        searchResults!.Count.Should().Be(1);
        results.TotalResultsCount.Should().Be(employerInterestsCount);
        searchResults.First().Validate(employerInterestSummaryDtoList.First());
        searchResults[0].SkillAreas[0].Should().Be(routeDtoList[0].RouteName);
    }

    [Fact]
    public async Task ExtendExpiry_Calls_Database_And_Returns_True_When_Update_Completed()
    {
        const int numberOfDaysToExtend = 84;
        const int expiryNotificationDays = 7;
        const int maximumExtensions = 10;
        var uniqueId = new Guid();
        var extensionResult = new ExtensionResultBuilder().Build();
        
        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<ExtensionResult>(dbConnection,
                Arg.Any<string>(),
                Arg.Any<object>(),
                commandType: CommandType.StoredProcedure)
            .Returns(new List<ExtensionResult> { extensionResult });

        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder()
            .Build();

        var repository = new EmployerInterestRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper);

        var result = await repository.ExtendExpiry(
            uniqueId, 
            numberOfDaysToExtend,
            expiryNotificationDays,
            maximumExtensions);

        result.Should().Be(extensionResult);

        await dbContextWrapper
            .Received(1)
            .QueryAsync<ExtensionResult>(dbConnection,
                "ExtendEmployerInterestExpiry",
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters),
                commandType: CommandType.StoredProcedure);
    }

    [Fact]
    public async Task ExtendExpiry_Calls_Database_And_Returns_False_When_No_Updates_Completed()
    {
        const int numberOfDaysToExtend = 84;
        const int expiryNotificationDays = 7;
        const int maximumExtensions = 10;
        var uniqueId = new Guid();
        var extensionResult = new ExtensionResultBuilder()
            .Build(false, 0);

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<ExtensionResult>(dbConnection,
                Arg.Any<string>(),
                Arg.Any<object>(),
                commandType: CommandType.StoredProcedure)
            .Returns(new List<ExtensionResult> { extensionResult });

        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder()
            .Build();

        var repository = new EmployerInterestRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper);

        var result = await repository.ExtendExpiry(
            uniqueId, 
            numberOfDaysToExtend,
            expiryNotificationDays,
            maximumExtensions);

        result.Success.Should().BeFalse();

        await dbContextWrapper
            .Received(1)
            .QueryAsync<ExtensionResult>(dbConnection,
                "ExtendEmployerInterestExpiry",
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters),
                commandType: CommandType.StoredProcedure);
    }

    [Fact]
    public async Task ExtendExpiry_Sets_Dynamic_Parameters()
    {
        const int numberOfDaysToExtend = 84;
        const int expiryNotificationDays = 7;
        const int maximumExtensions = 10;
        var uniqueId = new Guid();

        var (dbContextWrapper, _, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new EmployerInterestRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.ExtendExpiry(uniqueId, 
            numberOfDaysToExtend,
            expiryNotificationDays, 
            maximumExtensions);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();
        templates.GetDynamicTemplatesCount().Should().Be(4);
        templates.ContainsNameAndValue("uniqueId", uniqueId);
        templates.ContainsNameAndValue("numberOfDaysToExtend", numberOfDaysToExtend);
        templates.ContainsNameAndValue("expiryNotificationDays", expiryNotificationDays);
        templates.ContainsNameAndValue("maximumExtensions", maximumExtensions);
    }

    [Fact]
    public async Task UpdateExtensionEmailSentDate_Calls_Database()
    {
        const int id = 10;

        var (dbContextWrapper, dbConnection, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new EmployerInterestRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.UpdateExtensionEmailSentDate(id);

        await dbContextWrapper
            .Received(1)
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s =>
                    s.Contains("UPDATE dbo.EmployerInterest") &&
                    s.Contains("SET ExtensionEmailSentDate = GETUTCDATE(),") &&
                    s.Contains("ModifiedOn = GETUTCDATE()") &&
                    s.Contains("WHERE Id = @id")),
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters));
    }

    [Fact]
    public async Task UpdateExtensionEmailSentDate_Sets_Dynamic_Parameters()
    {
        const int id = 10;

        var (dbContextWrapper, _, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new EmployerInterestRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.UpdateExtensionEmailSentDate(id);
        
        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();
        templates.ContainsNameAndValue("id", id);
    }
}