using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Models;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Controllers;

public class QualificationsControllerTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(QualificationsController)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_BadParameters()
    {
        typeof(QualificationsController)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task GetQualifications_Returns_Expected_List()
    {
        var qualifications = new QualificationBuilder()
            .BuildList()
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.GetQualifications().Returns(qualifications);

        var controller = new QualificationsControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetQualifications();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as IEnumerable<Qualification>;
        results.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetQualifications_Returns_Expected_Value_For_One_Item()
    {
        var qualifications = new QualificationBuilder()
            .BuildList()
            .Take(1)
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.GetQualifications().Returns(qualifications);

        var controller = new QualificationsControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetQualifications();

        var results = ((result as OkObjectResult)?.Value as IEnumerable<Qualification>)?.ToList();
        results.Should().NotBeNullOrEmpty();
        results!.Count.Should().Be(1);

        results.Single().Id.Should().Be(qualifications.Single().Id);
        results.Single().Name.Should().Be(qualifications.Single().Name);
    }
}