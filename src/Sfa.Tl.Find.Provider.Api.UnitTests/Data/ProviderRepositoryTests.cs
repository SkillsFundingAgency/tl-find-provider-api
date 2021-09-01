using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Data
{
    public class ProviderRepositoryTests
    {
        [Fact]
        public void Constructor_Guards_Against_NullParameters()
        {
            typeof(ProviderRepository)
                .ShouldNotAcceptNullConstructorArguments();
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

            var repository = new ProviderRepositoryBuilder().Build(dbContextWrapper);

            await repository.Save(providers);

            await dbContextWrapper
                .Received(3)
                .QueryAsync<(string Change, int ChangeCount)>(
                    dbConnection,
                    Arg.Any<string>(),
                    Arg.Is<object>(o => o != null),
                    Arg.Is<IDbTransaction>(t => t != null),
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
        public async Task Search_Returns_Expected_List_For_Single_Result_Row()
        {
            var fromPostcodeLocation = new PostcodeLocationBuilder().BuildValidPostcodeLocation();

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
            
            var expectedResult = new ProviderSearchResultBuilder()
                .WithSearchOrigin(fromPostcodeLocation)
                .BuildListWithSingleItem()
                .First();

            var dbConnection = Substitute.For<IDbConnection>();
            var dbContextWrapper = Substitute.For<IDbContextWrapper>();
            dbContextWrapper
                .CreateConnection()
                .Returns(dbConnection);

            var callIndex = 0;

            await dbContextWrapper
                .QueryAsync(dbConnection,
                    "SearchProviders",
                    Arg.Do<Func<ProviderSearchResult, DeliveryYear, Qualification, ProviderSearchResult>>(
                        x =>
                        {
                            //TODO: Write a more complex test that deals with multiple results
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

            var repository = new ProviderRepositoryBuilder().Build(dbContextWrapper, dateTimeService);

            var searchResults = await repository.Search(fromPostcodeLocation, null, 0, 5);

            var searchResultsList = searchResults?.ToList();
            searchResultsList.Should().NotBeNull();
            searchResultsList!.Count.Should().Be(1);

            var firstResult = searchResultsList.First();

            firstResult.UkPrn.Should().Be(expectedResult.UkPrn);
            firstResult.ProviderName.Should().Be(expectedResult.ProviderName);
            firstResult.Postcode.Should().Be(expectedResult.Postcode);
            firstResult.LocationName.Should().Be(expectedResult.LocationName);
            firstResult.AddressLine1.Should().Be(expectedResult.AddressLine1);
            firstResult.AddressLine2.Should().Be(expectedResult.AddressLine2);
            firstResult.Town.Should().Be(expectedResult.Town);
            firstResult.County.Should().Be(expectedResult.County);
            firstResult.Email.Should().Be(expectedResult.Email);
            firstResult.Telephone.Should().Be(expectedResult.Telephone);
            firstResult.Website.Should().Be(expectedResult.Website);
            firstResult.Distance.Should().Be(expectedResult.Distance);
            firstResult.JourneyToLink.Should().Be(expectedResult.JourneyToLink);
            
            firstResult.DeliveryYears.Count.Should().Be(expectedResult.DeliveryYears.Count);
            var deliveryYear = firstResult.DeliveryYears.First();
            var expectedDeliveryYear = expectedResult.DeliveryYears.First();

            deliveryYear.Year.Should().Be(expectedDeliveryYear.Year);
            deliveryYear.IsAvailableNow.Should().Be(expectedDeliveryYear.IsAvailableNow);
            deliveryYear.Qualifications.Count.Should().Be(expectedDeliveryYear.Qualifications.Count);

            var qualification = deliveryYear.Qualifications.First();
            var expectedQualification = expectedDeliveryYear.Qualifications.First();

            qualification.Id.Should().Be(expectedQualification.Id);
            qualification.Name.Should().Be(expectedQualification.Name);
        }
    }
}
