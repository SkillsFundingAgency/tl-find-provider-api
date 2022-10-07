using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Data;

public class IndustryRepositoryTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(IndustryRepository)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task GetAll_Returns_Expected_List()
    {
        var industries = new IndustryBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<Industry>(dbConnection, 
                Arg.Any<string>())
            .Returns(industries);

        const string expectedSql =
            "SELECT Id, Name FROM dbo.Industry WHERE IsDeleted = 0 ORDER BY Name";

        var repository = new IndustryRepositoryBuilder().Build(dbContextWrapper);

        var results = (await repository.GetAll()).ToList();
        results.Should().BeEquivalentTo(industries);

        await dbContextWrapper
            .Received(1)
            .QueryAsync<Industry>(dbConnection,
                Arg.Is<string>(sql => sql == expectedSql));
    }
}