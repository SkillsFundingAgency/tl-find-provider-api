using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Models;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Repositories;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Data;

public class RouteRepositoryTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(RouteRepository)
            .ShouldNotAcceptNullConstructorArguments();
    }
        
    [Fact]
    public async Task GetAll_Returns_Expected_List()
    {
        var routes = new RouteBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<Route>(dbConnection, 
                "GetRoutes",
                Arg.Any<object>(),
                commandType: CommandType.StoredProcedure)
            .Returns(routes);

        var repository = new RouteRepositoryBuilder().Build(dbContextWrapper);

        var results = (await repository
            .GetAll(true))
            .ToList();

        results.Should().BeEquivalentTo(routes);

        await dbContextWrapper
            .Received(1)
            .QueryAsync<Route>(dbConnection,
                "GetRoutes",
                Arg.Any<object>(),
                commandType: CommandType.StoredProcedure
                );
    }
}