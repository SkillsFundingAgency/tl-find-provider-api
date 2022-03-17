using System;
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
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Models;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Policies;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Repositories;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Data;

public class ProviderRepositoryTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(ProviderRepository)
            .ShouldNotAcceptNullConstructorArguments();
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
            //.WithUpdates(0)
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

        var pollyPolicy = PollyPolicyBuilder.BuildPolicy();
        var pollyPolicyRegistry = PollyPolicyBuilder.BuildPolicyRegistry(pollyPolicy);

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
                Arg.Is<object>(o => o != null),
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
        var pollyPolicy = PollyPolicyBuilder.BuildPolicy();
        var pollyPolicyRegistry = PollyPolicyBuilder.BuildPolicyRegistry(pollyPolicy);

        var logger = Substitute.For<ILogger<ProviderRepository>>();

        var repository = new ProviderRepositoryBuilder()
            .Build(
                policyRegistry: pollyPolicyRegistry,
                logger: logger);

        await repository.Save(new List<Models.Provider>());

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
        var fromPostcodeLocation = PostcodeLocationBuilder.BuildValidPostcodeLocation();

        var expectedResult = new ProviderSearchResultBuilder()
            .BuildSingleSearchResultWithSearchOrigin(fromPostcodeLocation);

        var repository = await BuildRepositoryWithDataToSearchProviders();

        var searchResults = await repository.Search(fromPostcodeLocation, null, null, 0, 5, false);

        var searchResultsList = searchResults?.ToList();
        searchResultsList.Should().NotBeNull();
        searchResultsList!.Count.Should().Be(1);

        ValidateProviderSearchResult(searchResultsList.First(), expectedResult);
    }

    [Fact]
    public async Task Search_Merges_Additional_Data()
    {
        var fromPostcodeLocation = PostcodeLocationBuilder.BuildValidPostcodeLocation();

        var expectedResult = new ProviderSearchResultBuilder()
             .BuildSingleSearchResultWithSearchOrigin(fromPostcodeLocation);

        var repository = await BuildRepositoryWithDataToSearchProviders();

        var searchResults = await repository
            .Search(fromPostcodeLocation, null, null, 0, 5, true);

        var searchResultsList = searchResults?.ToList();
        searchResultsList.Should().NotBeNull();
        searchResultsList!.Count.Should().Be(1);

        ValidateProviderSearchResult(searchResultsList.First(), expectedResult);
    }

    private static async Task<IProviderRepository> BuildRepositoryWithDataToSearchProviders()
    {
        var providersPart = new ProviderSearchResultBuilder()
            .BuildProvidersPartOfListWithSingleItem()
            .Take(1)
            .ToList();
        var deliveryYearsPart = new ProviderSearchResultBuilder()
            .BuildDeliveryYearsPartOfListWithSingleItem()
            .Take(1)
            .ToList();
        var qualificationsPart = new ProviderSearchResultBuilder()
            .BuildQualificationsPartOfListWithSingleItem()
            .Take(1)
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        var callIndex = 0;

        await dbContextWrapper
            .QueryAsync(dbConnection,
                "SearchProviders",
                Arg.Do<Func<ProviderSearchResult, DeliveryYear, Qualification, ProviderSearchResult>>(
                    x =>
                    {
                        var p = providersPart[callIndex];
                        var d = deliveryYearsPart[callIndex];
                        var q = qualificationsPart[callIndex];
                        x.Invoke(p, d, q);

                        callIndex++;
                    }),
                Arg.Any<object>(),
                splitOn: Arg.Any<string>(),
                commandType: CommandType.StoredProcedure
            );

        var dateTimeService = Substitute.For<IDateTimeService>();
        dateTimeService.Today.Returns(DateTime.Parse("2021-09-01"));

        return new ProviderRepositoryBuilder().Build(dbContextWrapper, dateTimeService);
    }

    private static void ValidateProviderSearchResult(ProviderSearchResult result, ProviderSearchResult expected)
    {
        result.UkPrn.Should().Be(expected.UkPrn);
        result.ProviderName.Should().Be(expected.ProviderName);
        result.Postcode.Should().Be(expected.Postcode);
        result.LocationName.Should().Be(expected.LocationName);
        result.AddressLine1.Should().Be(expected.AddressLine1);
        result.AddressLine2.Should().Be(expected.AddressLine2);
        result.Town.Should().Be(expected.Town);
        result.County.Should().Be(expected.County);
        result.Email.Should().Be(expected.Email);
        result.Telephone.Should().Be(expected.Telephone);
        result.Website.Should().Be(expected.Website);
        result.Distance.Should().Be(expected.Distance);
        result.JourneyToLink.Should().Be(expected.JourneyToLink);

        result.DeliveryYears.Should().NotBeNull();
        result.DeliveryYears.Count.Should().Be(expected.DeliveryYears.Count);

        foreach (var deliveryYear in result.DeliveryYears)
        {
            var expectedDeliveryYear = expected.DeliveryYears.Single(dy => dy.Year == deliveryYear.Year);
            ValidateDeliveryYear(deliveryYear, expectedDeliveryYear);
        }
    }

    private static void ValidateDeliveryYear(DeliveryYear deliveryYear, DeliveryYear expected)
    {
        deliveryYear.Year.Should().Be(expected.Year);
        deliveryYear.IsAvailableNow.Should().Be(expected.IsAvailableNow);
        deliveryYear.Qualifications.Should().NotBeNull();
        deliveryYear.Qualifications.Count.Should().Be(expected.Qualifications.Count);

        foreach (var qualification in deliveryYear.Qualifications)
        {
            var expectedQualification = expected.Qualifications.Single(q => q.Id == qualification.Id);
            ValidateQualification(qualification, expectedQualification);
        }
    }

    private static void ValidateQualification(Qualification qualification, Qualification expected)
    {
        qualification.Id.Should().Be(expected.Id);
        qualification.Name.Should().Be(expected.Name);
    }
}