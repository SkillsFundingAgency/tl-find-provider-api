using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Controllers;

public class RoutesControllerTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(RoutesController)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_BadParameters()
    {
        typeof(RoutesController)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task GetRoutes_Returns_Expected_List()
    {
        var routes = new RouteBuilder()
            .BuildList()
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.GetRoutes().Returns(routes);

        var controller = new RoutesControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetRoutes();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as IEnumerable<Route>;
        results.Should().BeEquivalentTo(routes);
    }

    [Fact]
    public async Task GetRoutes_Returns_Expected_Value_For_One_Item()
    {
        var routes = new RouteBuilder()
            .BuildList()
            .Take(1)
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.GetRoutes().Returns(routes);

        var controller = new RoutesControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetRoutes();

        var results = ((result as OkObjectResult)?.Value as IEnumerable<Route>)?.ToList();
        results.Should().NotBeNullOrEmpty();
        results!.Count.Should().Be(1);

        results.Single().Id.Should().Be(routes.Single().Id);
        results.Single().Name.Should().Be(routes.Single().Name);
    }
}