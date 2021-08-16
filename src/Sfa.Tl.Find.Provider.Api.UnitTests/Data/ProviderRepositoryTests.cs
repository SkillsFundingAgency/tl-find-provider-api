using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Interfaces;
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
        public async Task Save_Returns_Expected_Result()
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
            var dbContextWrapper = Substitute.For<IDbContextWrapper>();
            dbContextWrapper
                .CreateConnection()
                .Returns(dbConnection);
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

            //TODO: Not sure this needs to return any results - log from repository
            var results = await repository.Save(providers);

            results.Should().NotBeNull();
            results.Inserted.Should().Be(10);
            results.Updated.Should().Be(5);
            results.Deleted.Should().Be(2);

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
        }

        [Fact]
        public async Task Search_Returns_Expected_List()
        {
            var providers = new ProviderBuilder()
                .BuildList()
                .ToList();

            var dbConnection = Substitute.For<IDbConnection>();
            var dbContextWrapper = Substitute.For<IDbContextWrapper>();
            dbContextWrapper
                .CreateConnection()
                .Returns(dbConnection);
            dbContextWrapper
                .QueryAsync<Models.Provider>(dbConnection,
                    "SearchProviders",
                    Arg.Any<object>(),
                    commandType: CommandType.StoredProcedure
                )
                .Returns(providers);

            var repository = new ProviderRepositoryBuilder().Build(dbContextWrapper);

            var fromPostcodeLocation = new PostcodeLocationBuilder().BuildValidPostcodeLocation();

            var searchResults = await repository.Search(fromPostcodeLocation, null, 0, 0);

            var searchResultsList = searchResults.ToList();
            //TODO: Refactor to proper search results and implement asserts
            //searchResultsList.Count.Should().Be(providers.Count);
            //searchResultsList.Should().BeEquivalentTo(providers);
        }
    }
}
