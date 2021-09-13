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
using Sfa.Tl.Find.Provider.Api.Models.Exceptions;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Controllers
{
    public class FindProvidersControllerTests
    {
        private const string TestPostcode = "AB1 2XY";
        private const string InvalidPostcode = "CV99 XXX";
        private const int TestQualificationId = 51;
        private const int TestPage = 3;
        private const int TestPageSize = Constants.DefaultPageSize + 10;

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
            var dataService = Substitute.For<IProviderDataService>();
            dataService.GetQualifications().Returns(new QualificationBuilder().BuildList());

            var controller = new FindProvidersControllerBuilder().Build(dataService);

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
            var qualifications = new QualificationBuilder().BuildList().Take(1).ToList();

            var dataService = Substitute.For<IProviderDataService>();
            dataService.GetQualifications().Returns(qualifications);

            var controller = new FindProvidersControllerBuilder().Build(dataService);

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
            var dataService = Substitute.For<IProviderDataService>();
            dataService.FindProviders(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(new ProviderSearchResponseBuilder()
                    .BuildWithMultipleSearchResults());

            var controller = new FindProvidersControllerBuilder().Build(dataService);

            await controller.GetProviders(TestPostcode);

            await dataService
                .Received()
                .FindProviders(Arg.Is<string>(p => p == TestPostcode),
                    Arg.Is<int?>(q => q == null),
                    Arg.Is<int>(p => p == 0),
                    Arg.Is<int>(s => s == Constants.DefaultPageSize));
        }

        [Fact]
        public async Task GetProviders_Passes_QualificationId_And_Default_Parameters()
        {
            var dataService = Substitute.For<IProviderDataService>();
            dataService.FindProviders(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(new ProviderSearchResponseBuilder()
                    .BuildWithMultipleSearchResults());

            var controller = new FindProvidersControllerBuilder().Build(dataService);

            await controller.GetProviders(TestPostcode, TestQualificationId);

            await dataService
                .Received()
                .FindProviders(Arg.Is<string>(p => p == TestPostcode),
                    Arg.Is<int?>(q => q == TestQualificationId),
                    Arg.Is<int>(p => p == 0),
                    Arg.Is<int>(s => s == Constants.DefaultPageSize));
        }

        [Fact]
        public async Task GetProviders_Passes_QualificationId_And_Page_And_Default_Parameters()
        {
            var dataService = Substitute.For<IProviderDataService>();
            dataService.FindProviders(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(new ProviderSearchResponseBuilder()
                    .BuildWithMultipleSearchResults());

            var controller = new FindProvidersControllerBuilder().Build(dataService);

            await controller.GetProviders(TestPostcode, TestQualificationId, TestPage);

            await dataService
                .Received()
                .FindProviders(Arg.Is<string>(p => p == TestPostcode),
                    Arg.Is<int?>(q => q == TestQualificationId),
                    Arg.Is<int>(p => p == TestPage),
                    Arg.Is<int>(s => s == Constants.DefaultPageSize));
        }

        [Fact]
        public async Task GetProviders_Passes_All_Parameters()
        {
            var dataService = Substitute.For<IProviderDataService>();
            dataService.FindProviders(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(new ProviderSearchResponseBuilder()
                    .BuildWithMultipleSearchResults());

            var controller = new FindProvidersControllerBuilder().Build(dataService);

            await controller.GetProviders(TestPostcode, TestQualificationId, TestPage, TestPageSize);

            await dataService
                .Received()
                .FindProviders(Arg.Is<string>(p => p == TestPostcode),
                    Arg.Is<int?>(q => q == TestQualificationId),
                    Arg.Is<int>(p => p == TestPage),
                    Arg.Is<int>(s => s == TestPageSize));
        }

        [Fact]
        public async Task GetProviders_Returns_Expected_List_Of_Search_Results()
        {
            var fromPostcodeLocation = new PostcodeLocationBuilder()
                .BuildValidPostcodeLocation();

            var dataService = Substitute.For<IProviderDataService>();
            dataService.FindProviders(fromPostcodeLocation.Postcode)
                .Returns(new ProviderSearchResponseBuilder()
                    .BuildWithMultipleSearchResults());

            var controller = new FindProvidersControllerBuilder().Build(dataService);

            var result = await controller.GetProviders(fromPostcodeLocation.Postcode);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            var results = okResult.Value as ProviderSearchResponse;
            results!.Postcode.Should().Be(fromPostcodeLocation.Postcode);
            results.SearchResults.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetProviders_Returns_Expected_Value_For_One_Search_Result()
        {
            var fromPostcodeLocation = new PostcodeLocationBuilder()
                .BuildValidPostcodeLocation();

            var providerSearchResponse = new ProviderSearchResponseBuilder()
                .WithSearchOrigin(fromPostcodeLocation)
                .BuildWithSingleSearchResult();

            var dataService = Substitute.For<IProviderDataService>();
            dataService.FindProviders(fromPostcodeLocation.Postcode)
                .Returns(providerSearchResponse);

            var controller = new FindProvidersControllerBuilder().Build(dataService);

            var result = await controller.GetProviders(fromPostcodeLocation.Postcode);

            var results = (result as OkObjectResult)?.Value 
                as ProviderSearchResponse;

            results.Should().NotBeNull();
            results!.Postcode.Should().Be(fromPostcodeLocation.Postcode);
            results.SearchResults.Should().NotBeNullOrEmpty();

            results.SearchResults.Count().Should().Be(1);
            results.SearchResults.Should().BeEquivalentTo(providerSearchResponse.SearchResults);
        }

        [Fact]
        public async Task GetProviders_Returns_Not_Found_Result_For_Invalid_Postcode()
        {
            var dataService = Substitute.For<IProviderDataService>();
            dataService.FindProviders(InvalidPostcode)
                .Throws(new PostcodeNotFoundException(InvalidPostcode));

            var controller = new FindProvidersControllerBuilder().Build(dataService);

            var result = await controller.GetProviders(InvalidPostcode);

            result.Should().BeOfType(typeof(NotFoundObjectResult));
            var notFoundObjectResult = result as NotFoundObjectResult;

            var message = notFoundObjectResult!.Value as string;
            message.Should().Be($"Postcode {InvalidPostcode} was not found");
        }

        [Fact]
        public async Task GetProviders_Returns_Error_Result_For_Internal_Error()
        {
            var dataService = Substitute.For<IProviderDataService>();
            dataService.FindProviders(InvalidPostcode)
                .Throws(new Exception("Test exception"));

            var controller = new FindProvidersControllerBuilder().Build(dataService);

            var result = await controller.GetProviders(InvalidPostcode);

            result.Should().BeOfType(typeof(StatusCodeResult));
            var statusCodeResult = result as StatusCodeResult;

            statusCodeResult!.StatusCode.Should().Be(500);
            statusCodeResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}
