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
    public class QualificationRepositoryTests
    {
        [Fact]
        public void Constructor_Guards_Against_NullParameters()
        {
            typeof(QualificationRepository)
                .ShouldNotAcceptNullConstructorArguments();
        }
        
        [Fact]
        public async Task GetAll_Returns_Expected_List()
        {
            var dbConnection = Substitute.For<IDbConnection>();
            var dbContextWrapper = Substitute.For<IDbContextWrapper>();
            dbContextWrapper
                .CreateConnection()
                .Returns(dbConnection);
            dbContextWrapper
                .QueryAsync<Qualification>(dbConnection, Arg.Any<string>())
                .Returns(new QualificationBuilder().BuildList());

            var repository = new QualificationRepositoryBuilder().Build(dbContextWrapper);

            var results = await repository.GetAll();
            results.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Save_Returns_Expected_Result()
        {
            var qualifications = new QualificationBuilder()
                .BuildList()
                .ToList();

            var dbConnection = Substitute.For<IDbConnection>();
            var dbContextWrapper = Substitute.For<IDbContextWrapper>();
            dbContextWrapper
                .CreateConnection()
                .Returns(dbConnection);
            dbContextWrapper
                .ExecuteAsync(dbConnection, 
                    "UpdateQualifications",
                    Arg.Any<object>(),
                    commandType: CommandType.StoredProcedure
                    )
                .Returns(qualifications.Count);

            var repository = new QualificationRepositoryBuilder().Build(dbContextWrapper);

            var results = await repository.Save(qualifications);

            results.Should().NotBeNull();
            results.Inserted.Should().Be(qualifications.Count);
            results.Updated.Should().Be(0);
            results.Deleted.Should().Be(0);
        }
    }
}
