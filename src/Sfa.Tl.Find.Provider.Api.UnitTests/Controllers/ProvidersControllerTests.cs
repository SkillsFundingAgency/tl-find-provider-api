using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Controllers;

public class ProvidersControllerTests
{
    private const string TestPostcode = "CV1 2WT";
    private const double TestLatitude = 52.400997;
    private const double TestLongitude = -1.508122;
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
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(ProvidersController)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(ProvidersController)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task GetAllProviders_Returns_Expected_List()
    {
        var providers = new ProviderDetailBuilder()
            .BuildList()
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.GetAllProviders()
            .Returns(new ProviderDetailResponse
            {
                Providers = providers
            });

        var controller = new ProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetAllProviderData();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as ProviderDetailResponse;
        results.Should().NotBeNull();
        results!.Providers.Should().BeEquivalentTo(providers);
    }

    [Fact]
    public async Task GetProviders_Passes_Default_Parameters()
    {
        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.FindProviders(Arg.Any<string>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(new ProviderSearchResponseBuilder()
                .BuildWithMultipleSearchResults());

        var controller = new ProvidersControllerBuilder()
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

        var controller = new ProvidersControllerBuilder()
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

        var controller = new ProvidersControllerBuilder()
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

        var controller = new ProvidersControllerBuilder()
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

        var controller = new ProvidersControllerBuilder()
            .Build(providerDataService);

        await controller.GetProviders(TestPostcode, null, null, _testRouteIds, _testQualificationIds, TestPage, TestPageSize);

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

        var controller = new ProvidersControllerBuilder()
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
    public async Task GetProviders_Returns_Expected_Available_Results_Count()
    {
        const int availableResults = 20;
        var fromGeoLocation = GeoLocationBuilder.BuildValidPostcodeLocation();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.FindProviders(fromGeoLocation.Location)
            .Returns(new ProviderSearchResponseBuilder()
                .WithTotalSearchResults(availableResults)
                .BuildWithMultipleSearchResults());

        var controller = new ProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetProviders(fromGeoLocation.Location);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as ProviderSearchResponse;
        results!.TotalResults.Should().Be(availableResults);
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

        var controller = new ProvidersControllerBuilder()
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

        var controller = new ProvidersControllerBuilder()
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

        var controller = new ProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetProviders();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as ProviderSearchResponse;
        results!.Error.Should().Be("Either search term or both lat/long required.");
    }

    [Fact]
    public async Task GetProviders_Validates_Empty_Postcode()
    {
        var providerDataService = Substitute.For<IProviderDataService>();

        var controller = new ProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetProviders("");

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as ProviderSearchResponse;
        results!.Error.Should().Be("Either search term or both lat/long required.");
    }

    [Fact]
    public async Task GetProviders_Validates_Illegal_Postcode_Characters()
    {
        var providerDataService = Substitute.For<IProviderDataService>();

        var controller = new ProvidersControllerBuilder()
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

        var controller = new ProvidersControllerBuilder()
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

        var controller = new ProvidersControllerBuilder()
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

        var controller = new ProvidersControllerBuilder()
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

        var controller = new ProvidersControllerBuilder()
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

        var controller = new ProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetProviders(InvalidPostcode);

        result.Should().BeOfType(typeof(StatusCodeResult));
        var statusCodeResult = result as StatusCodeResult;

        statusCodeResult!.StatusCode.Should().Be(500);
        statusCodeResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task GetProviders_By_LatLong_Passes_All_Parameters()
    {
        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.FindProviders(Arg.Any<double>(), Arg.Any<double>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(new ProviderSearchResponseBuilder()
                .BuildWithMultipleSearchResults());

        var controller = new ProvidersControllerBuilder()
            .Build(providerDataService);

        await controller.GetProviders(null, TestLatitude, TestLongitude, _testRouteIds, _testQualificationIds, TestPage, TestPageSize);

        await providerDataService
            .Received()
            .FindProviders(
                // ReSharper disable CompareOfFloatsByEqualityOperator
                Arg.Is<double>(l => l == TestLatitude),
                Arg.Is<double>(l => l == TestLongitude),
                // ReSharper restore CompareOfFloatsByEqualityOperator
                Arg.Is<IList<int>>(r => r.ListIsEquivalentTo(_testRouteIds)),
                Arg.Is<IList<int>>(q => q.ListIsEquivalentTo(_testQualificationIds)),
                Arg.Is<int>(p => p == TestPage),
                Arg.Is<int>(s => s == TestPageSize));
    }

    [Fact]
    public async Task GetProviders_Validates_Postcode_Or_Lat_Long_Exclusive()
    {
        var providerDataService = Substitute.For<IProviderDataService>();

        var controller = new ProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetProviders(
            TestPostcode, 
            TestLatitude, TestLongitude, 
            _testRouteIds, 
            _testQualificationIds, 
            TestPage, 
            TestPageSize);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as ProviderSearchResponse;
        results!.Error.Should().Be("Either search term or lat/long required, but not both.");
    }

    [Fact]
    public async Task GetProviders_Validates_Allow_Postcode_With_No_Lat_Long()
    {
        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.FindProviders(TestPostcode, Arg.Any<IList<int>>(), Arg.Any<IList<int>>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(new ProviderSearchResponseBuilder()
                .BuildWithMultipleSearchResults());

        var controller = new ProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetProviders(TestPostcode, null, null, _testRouteIds, _testQualificationIds, TestPage, TestPageSize);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as ProviderSearchResponse;
        results!.Error.Should().BeNull();
    }

    [Fact]
    public async Task GetProviders_Validates_Allow_Lat_Long_With_No_Postcode()
    {
        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.FindProviders(Arg.Any<double>(), Arg.Any<double>(), Arg.Any<List<int>>(), Arg.Any<List<int>>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(new ProviderSearchResponseBuilder()
                .BuildWithMultipleSearchResults());

        var controller = new ProvidersControllerBuilder()
            .Build(providerDataService);

        var result = await controller.GetProviders(null, TestLatitude, TestLongitude, _testRouteIds, _testQualificationIds, TestPage, TestPageSize);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as ProviderSearchResponse;
        results!.Error.Should().BeNull();
    }

    [Fact]
    public async Task GetProviderDataAsCsv_Returns_Expected_Result()
    {
        var bytes = new byte[] { 104, 101, 108 , 108, 111 };

        var dateTimeService = Substitute.For<IDateTimeService>();
        dateTimeService.Today.Returns(DateTime.Parse("2022-08-19"));
        const string expectedFileName = "All T Level providers August 2022.csv";

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.GetCsv()
            .Returns(bytes);

        var controller = new ProvidersControllerBuilder()
            .Build(providerDataService, dateTimeService);

        var result = await controller.GetProviderDataAsCsv();

        var fileContentResult = result as FileContentResult;
        fileContentResult.Should().NotBeNull();
        fileContentResult!.ContentType.Should().Be("text/csv");
        fileContentResult!.FileDownloadName.Should().Be(expectedFileName);
        fileContentResult!.FileContents.Should().BeEquivalentTo(bytes);
    }

    [Fact]
    public async Task GetProviderDataCsvFileInfo_Returns_Expected_Result()
    {
        var bytes = new byte[] { 104, 101, 108, 108, 111 };

        var dateTimeService = Substitute.For<IDateTimeService>();
        dateTimeService.Today.Returns(DateTime.Parse("2022-08-19"));
        const string expectedFormattedFileDate = "August 2022";

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.GetCsv()
            .Returns(bytes);

        var controller = new ProvidersControllerBuilder()
            .Build(providerDataService,
                dateTimeService);

        var result = await controller.GetProviderDataCsvFileInfo();
        
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var info = okResult.Value as ProviderDataDownloadInfoResponse;
        info.Should().NotBeNull();
        info!.FileSize.Should().Be(bytes.Length);
        info!.FormattedFileDate.Should().Be(expectedFormattedFileDate);
    }

    [Fact]
    public async Task GetProviderDataCsvFileInfo_Returns_Expected_Result_From_Cache()
    {
        const string formattedDate = "August 2022";
        const int fileSize = 10101;

        //var bytes = new byte[] { 104, 101, 108, 108, 111 };
        var cachedInfo = new ProviderDataDownloadInfoResponse
            {
                FormattedFileDate = formattedDate,
                FileSize = fileSize
            };

        var cache = Substitute.For<IMemoryCache>();
        cache.TryGetValue(Arg.Any<string>(), out Arg.Any<IList<Qualification>>())
            .Returns(x =>
            {
                if ((string)x[0] == CacheKeys.ProviderDataDownloadInfoKey)
                {
                    x[1] = cachedInfo;
                    return true;
                }

                return false;
            });

        var dateTimeService = Substitute.For<IDateTimeService>();
        dateTimeService.Today.Returns(DateTime.Parse("2022-08-19"));

        var providerDataService = Substitute.For<IProviderDataService>();
 
        var controller = new ProvidersControllerBuilder()
            .Build(providerDataService,
                dateTimeService,
                cache);

        var result = await controller.GetProviderDataCsvFileInfo();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var info = okResult.Value as ProviderDataDownloadInfoResponse;
        info.Should().NotBeNull();
        info!.FileSize.Should().Be(fileSize);
        info!.FormattedFileDate.Should().Be(formattedDate);

        info.Should().Be(cachedInfo);
        info.Should().BeEquivalentTo(cachedInfo);

        await providerDataService
            .DidNotReceive()
            .GetCsv();
    }
}