using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Controllers;

public class IndustriesControllerTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(IndustriesController)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(IndustriesController)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task GetIndustries_Returns_Expected_List()
    {
        var industries = new IndustryBuilder()
            .BuildList()
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.GetIndustries().Returns(industries);

        var controller = new IndustriesControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetIndustries();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as IEnumerable<Industry>;
        results.Should().BeEquivalentTo(industries);
    }

    [Fact]
    public async Task GetGetIndustries_Returns_Expected_Value_For_One_Item()
    {
        var industries = new IndustryBuilder()
            .BuildList()
            .Take(1)
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.GetIndustries().Returns(industries);

        var controller = new IndustriesControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetIndustries();

        var results = ((result as OkObjectResult)?.Value as IEnumerable<Industry>)?.ToList();
        results.Should().NotBeNullOrEmpty();
        results!.Count.Should().Be(1);

        results.Single().Id.Should().Be(industries.Single().Id);
        results.Single().Name.Should().Be(industries.Single().Name);
    }
}