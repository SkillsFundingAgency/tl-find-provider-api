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
        private const string TestPostcode = "CV1 2WT";

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

            var changeResult = new DataBaseChangeResultBuilder()
                .WithInserts(10)
                .WithUpdates(5)
                .WithDeletes(2)
                .Build();

            var dbConnection = Substitute.For<IDbConnection>();
            var dbContextWrapper = Substitute.For<IDbContextWrapper>();
            dbContextWrapper
                .CreateConnection()
                .Returns(dbConnection);
            dbContextWrapper
                .QueryAsync<(string Change, int ChangeCount)>(dbConnection,
                    "UpdateProviders",
                    Arg.Any<object>(),
                    commandType: CommandType.StoredProcedure
                )
                .Returns(changeResult);

            var repository = new ProviderRepositoryBuilder().Build(dbContextWrapper);

            var results = await repository.Save(providers);

            results.Should().NotBeNull();
            results.Inserted.Should().Be(10);
            results.Updated.Should().Be(5);
            results.Deleted.Should().Be(2);
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
            searchResultsList.Count.Should().Be(providers.Count);
            searchResultsList.Should().BeEquivalentTo(providers);
        }
    }
}
