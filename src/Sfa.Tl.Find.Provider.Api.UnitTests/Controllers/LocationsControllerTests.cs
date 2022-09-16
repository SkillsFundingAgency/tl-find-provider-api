using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Controllers;

public class LocationsControllerTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(LocationsController)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(LocationsController)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task SearchTowns_Returns_Expected_List()
    {
        const string searchTerm = "Coventry";
        var towns = new TownBuilder()
            .BuildList()
            .ToList();

        var townDataService = Substitute.For<ITownDataService>();
        townDataService.Search(searchTerm)
            .Returns(towns);

        var controller = new LocationsControllerBuilder()
            .Build(townDataService);

        var result = await controller.Search(searchTerm);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as IEnumerable<Town>;
        results.Should().BeEquivalentTo(towns);
    }
}