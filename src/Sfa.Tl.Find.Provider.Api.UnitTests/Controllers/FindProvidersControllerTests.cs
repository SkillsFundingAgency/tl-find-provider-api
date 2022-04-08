using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Models;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Controllers;

public class FindProvidersControllerTests
{
    private const string TestPostcode = "CV1 2WT";
    private const string InvalidPostcode = "CV99 XXX";
    private const string PostcodeWithInitialNonLetter = "1V1 2WT";
    private const string PostcodeWithIllegalCharacters = "CV99 XX$";
    private const string PostcodeWithTooManyCharacters = "CV99 XG2 Z15";
    private const string PostcodeWithTooFewCharacters = "C";
    private const string PostcodeWithMinimumCharacters = "L1";
    private const int TestPage = 3;
    private const int TestPageSize = Constants.DefaultPageSize + 10;

    private readonly IList<int> _testRouteIds = new List<int> { 6, 7 };
    private readonly IList<int> _testQualificationIds = new List<int> { 37, 40, 51 };

    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(FindProvidersController)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_BadParameters()
    {
        typeof(FindProvidersController)
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

        var controller = new FindProvidersControllerBuilder()
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

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetQualifications();

        var results = ((result as OkObjectResult)?.Value as IEnumerable<Qualification>)?.ToList();
        results.Should().NotBeNullOrEmpty();
        results!.Count.Should().Be(1);

        results.Single().Id.Should().Be(qualifications.Single().Id);
        results.Single().Name.Should().Be(qualifications.Single().Name);
    }

    [Fact]
    public async Task GetProviders_Passes_Default_Parameters()
    {
        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.FindProviders(Arg.Any<string>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(new ProviderSearchResponseBuilder()
                .BuildWithMultipleSearchResults());

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        await controller.GetProviders(TestPostcode);

        await providerDataService
            .Received()
            .FindProviders(Arg.Is<string>(p => p == TestPostcode),
                Arg.Is<IList<int>>(q => q == null),
                Arg.Is<IList<int>>(q => q == null),
                Arg.Is<int>(p => p == 0),
                Arg.Is<int>(s => s == Constants.DefaultPageSize));
    }

    [Fact]
    public async Task GetProviders_Passes_QualificationIds_And_Default_Parameters()
    {
        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.FindProviders(Arg.Any<string>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(new ProviderSearchResponseBuilder()
                .BuildWithMultipleSearchResults());

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        await controller.GetProviders(TestPostcode, qualificationIds: _testQualificationIds);

        await providerDataService
            .Received()
            .FindProviders(Arg.Is<string>(p => p == TestPostcode),
                Arg.Is<IList<int>>(r => r == null),
                Arg.Is<IList<int>>(q => q.ListIsEquivalentTo(_testQualificationIds)),
                Arg.Is<int>(p => p == 0),
                Arg.Is<int>(s => s == Constants.DefaultPageSize));
    }

    [Fact]
    public async Task GetProviders_Passes_QualificationIds_And_Page_And_Default_Parameters()
    {
        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.FindProviders(Arg.Any<string>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(new ProviderSearchResponseBuilder()
                .BuildWithMultipleSearchResults());

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        await controller.GetProviders(TestPostcode, qualificationIds: _testQualificationIds, page: TestPage);

        await providerDataService
            .Received()
            .FindProviders(Arg.Is<string>(p => p == TestPostcode),
                Arg.Is<IList<int>>(r => r == null),
                Arg.Is<IList<int>>(q => q.ListIsEquivalentTo(_testQualificationIds)),
                Arg.Is<int>(p => p == TestPage),
                Arg.Is<int>(s => s == Constants.DefaultPageSize));
    }

    [Fact]
    public async Task GetProviders_Passes_RouteIds_And_Default_Parameters()
    {
        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.FindProviders(Arg.Any<string>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(new ProviderSearchResponseBuilder()
                .BuildWithMultipleSearchResults());

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        await controller.GetProviders(TestPostcode, routeIds: _testRouteIds);

        await providerDataService
            .Received()
            .FindProviders(Arg.Is<string>(p => p == TestPostcode),
                Arg.Is<IList<int>>(r => r.ListIsEquivalentTo(_testRouteIds)),
                Arg.Is<IList<int>>(q => q == null),
                Arg.Is<int>(p => p == 0),
                Arg.Is<int>(s => s == Constants.DefaultPageSize));
    }

    [Fact]
    public async Task GetProviders_Passes_All_Parameters()
    {
        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.FindProviders(Arg.Any<string>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(new ProviderSearchResponseBuilder()
                .BuildWithMultipleSearchResults());

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        await controller.GetProviders(TestPostcode, _testRouteIds, _testQualificationIds, TestPage, TestPageSize);

        await providerDataService
            .Received()
            .FindProviders(Arg.Is<string>(p => p == TestPostcode),
                Arg.Is<IList<int>>(r => r.ListIsEquivalentTo(_testRouteIds)),
                Arg.Is<IList<int>>(q => q.ListIsEquivalentTo(_testQualificationIds)),
                Arg.Is<int>(p => p == TestPage),
                Arg.Is<int>(s => s == TestPageSize));
    }

    [Fact]
    public async Task GetProviders_Returns_Expected_List_Of_Search_Results()
    {
        var fromGeoLocation = GeoLocationBuilder.BuildValidPostcodeLocation();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.FindProviders(fromGeoLocation.Location)
            .Returns(new ProviderSearchResponseBuilder()
                .BuildWithMultipleSearchResults());

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetProviders(fromGeoLocation.Location);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as ProviderSearchResponse;
        results!.SearchTerm.Should().Be(fromGeoLocation.Location);
        results.SearchResults.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetProviders_Returns_Expected_Value_For_One_Search_Result()
    {
        var fromGeoLocation = GeoLocationBuilder.BuildValidPostcodeLocation();

        var providerSearchResponse = new ProviderSearchResponseBuilder()
            .WithSearchOrigin(fromGeoLocation)
            .BuildWithSingleSearchResult();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.FindProviders(fromGeoLocation.Location)
            .Returns(providerSearchResponse);

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetProviders(fromGeoLocation.Location);

        var results = (result as OkObjectResult)?.Value
            as ProviderSearchResponse;

        results.Should().NotBeNull();
        results!.SearchTerm.Should().Be(fromGeoLocation.Location);
        results.SearchResults.Should().NotBeNullOrEmpty();

        results.SearchResults.Count().Should().Be(1);
        results.SearchResults.Should().BeEquivalentTo(providerSearchResponse.SearchResults);
    }

    [Fact]
    public async Task GetProviders_Returns_Not_Found_Result_For_Invalid_Postcode()
    {
        var errorMessage = $"Postcode {InvalidPostcode} was not found";

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.FindProviders(InvalidPostcode)
            .Returns(new ProviderSearchResponseBuilder()
                .BuildErrorResponse(errorMessage));

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetProviders(InvalidPostcode);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as ProviderSearchResponse;
        results!.Error.Should().Be(errorMessage);
    }

    [Fact]
    public async Task GetProviders_Validates_Null_Postcode()
    {
        var providerDataService = Substitute.For<IProviderDataService>();

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetProviders(null);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as ProviderSearchResponse;
        results!.Error.Should().Be("The search term is required.");
    }

    [Fact]
    public async Task GetProviders_Validates_Empty_Postcode()
    {
        var providerDataService = Substitute.For<IProviderDataService>();

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetProviders("");

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as ProviderSearchResponse;
        results!.Error.Should().Be("The search term is required.");
    }

    [Fact]
    public async Task GetProviders_Validates_Illegal_Postcode_Characters()
    {
        var providerDataService = Substitute.For<IProviderDataService>();

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetProviders(PostcodeWithIllegalCharacters);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as ProviderSearchResponse;
        results!.Error.Should().Be("The search term must start with a letter and contain only letters, numbers, and spaces.");
    }

    [Fact]
    public async Task GetProviders_Validates_Postcode_Does_Not_Start_With_Letter()
    {
        var providerDataService = Substitute.For<IProviderDataService>();

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetProviders(PostcodeWithInitialNonLetter);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as ProviderSearchResponse;
        results!.Error.Should().Be("The search term must start with a letter and contain only letters, numbers, and spaces.");
    }

    [Fact]
    public async Task GetProviders_Validates_Postcode_Maximum_Length()
    {
        var providerDataService = Substitute.For<IProviderDataService>();

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetProviders(PostcodeWithTooManyCharacters);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as ProviderSearchResponse;
        results!.Error.Should().Be("The postcode must be no more than 8 characters.");
    }

    [Fact]
    public async Task GetProviders_Validates_Postcode_Minimum_Length()
    {
        var providerDataService = Substitute.For<IProviderDataService>();

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetProviders(PostcodeWithTooFewCharacters);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as ProviderSearchResponse;
        results!.Error.Should().Be("The search term must be at least 2 characters.");
    }

    [Fact]
    public async Task GetProviders_Allows_Postcode_With_Minimum_Length()
    {
        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.FindProviders(PostcodeWithMinimumCharacters, Arg.Any<IList<int>>(), Arg.Any<IList<int>>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(new ProviderSearchResponseBuilder()
                .BuildWithSingleSearchResult());

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetProviders(PostcodeWithMinimumCharacters);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as ProviderSearchResponse;
        results!.Error.Should().BeNull();
    }

    [Fact]
    public async Task GetProviders_Returns_Error_Result_For_Internal_Error()
    {
        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.FindProviders(InvalidPostcode)
            .Throws(new Exception("Test exception"));

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetProviders(InvalidPostcode);

        result.Should().BeOfType(typeof(StatusCodeResult));
        var statusCodeResult = result as StatusCodeResult;

        statusCodeResult!.StatusCode.Should().Be(500);
        statusCodeResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task GetRoutes_Returns_Expected_List()
    {
        var routes = new RouteBuilder()
            .BuildList()
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.GetRoutes().Returns(routes);

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetRoutes();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as IEnumerable<Route>;
        results.Should().NotBeNullOrEmpty();
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

        var controller = new FindProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetRoutes();

        var results = ((result as OkObjectResult)?.Value as IEnumerable<Route>)?.ToList();
        results.Should().NotBeNullOrEmpty();
        results!.Count.Should().Be(1);

        results.Single().Id.Should().Be(routes.Single().Id);
        results.Single().Name.Should().Be(routes.Single().Name);
    }
}