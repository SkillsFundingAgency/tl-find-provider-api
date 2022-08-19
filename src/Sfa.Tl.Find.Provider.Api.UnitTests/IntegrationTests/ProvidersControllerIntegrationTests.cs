using System.Net;
using FluentAssertions;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.IntegrationTests;

public class ProvidersControllerIntegrationTests : IClassFixture<TestServerFactory<FakeStartup>>
{
    private readonly TestServerFactory<FakeStartup> _fixture;

    public ProvidersControllerIntegrationTests(TestServerFactory<FakeStartup> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetProviders_Returns_OK_Result_For_Valid_Url_With_Postcode()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v3/providers?searchTerm=CV1+2WT&qualificationId=40&routeId=5&page=0&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProviders_Returns_OK_Result_For_Valid_Url_With_Town_Name()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v3/providers?searchTerm=coventry&qualificationId=40&routeId=5&page=0&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProviders_Returns_OK_Result_For_Valid_Lat_Long_Url()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v3/providers?lat=52.400997&lon=-1.508122&page=0&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProviders_Returns_OK_Result_For_Short_Postcode_Url()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v3/providers?searchTerm=L1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var searchResponse = await response.Content.DeserializeFromHttpContent<ProviderSearchResponse>();
        searchResponse.Should().NotBeNull();
        searchResponse!.Error.Should().BeNull();
    }

    [Fact]
    public async Task GetProviders_Returns_Error_Message_Result_For_Missing_Postcode()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v3/providers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var searchResponse = await response.Content.DeserializeFromHttpContent<ProviderSearchResponse>();
        searchResponse.Should().NotBeNull();
        searchResponse!.Error.Should().Be("Either search term or both lat/long required.");
    }

    [Fact]
    public async Task GetProviders_Returns_Error_Message_Result_For_Postcode_Too_Short()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v3/providers?searchTerm=A");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var searchResponse = await response.Content.DeserializeFromHttpContent<ProviderSearchResponse>();
        searchResponse.Should().NotBeNull();
        searchResponse!.Error.Should().Be("The search term must be at least 2 characters.");
    }

    [Fact]
    public async Task GetProviders_Returns_Bad_Request_Result_For_Zero_PageSize()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v3/providers?searchTerm=CV1+2WT&page=0&pageSize=0");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await response.Content.ValidateProblemDetails(
            ("pageSize", "The pageSize field must be at least one."));
    }

    [Fact]
    public async Task GetProviders_Returns_Error_Message_Result_For_Postcode_With_Illegal_Characters()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v3/providers?searchTerm=CV1+2WT£");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var searchResponse = await response.Content.DeserializeFromHttpContent<ProviderSearchResponse>();
        searchResponse.Should().NotBeNull();
        searchResponse!.Error.Should().Be("The search term must start with a letter and contain only letters, numbers, and spaces.");
    }

    [Fact]
    public async Task GetProviders_Returns_Error_Message_Result_For_Postcode_Starting_With_Number()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v3/providers?searchTerm=2V1+2WT");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var searchResponse = await response.Content.DeserializeFromHttpContent<ProviderSearchResponse>();
        searchResponse.Should().NotBeNull();
        searchResponse!.Error.Should().Be("The search term must start with a letter and contain only letters, numbers, and spaces.");
    }

    [Fact]
    public async Task GetProviders_Returns_Bad_Request_Result_For_Negative_Page()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v3/providers?searchTerm=CV1+2WT&page=-1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await response.Content.ValidateProblemDetails(
            ("page", "The page field must be zero or greater."));
    }

    [Fact]
    public async Task GetProviders_Returns_Bad_Request_Result_With_All_Errors()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v3/providers?qualificationId=40&page=-1&pageSize=0");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await response.Content.ValidateProblemDetails(
            ("pageSize", "The pageSize field must be at least one."),
            ("page", "The page field must be zero or greater."));
    }

    [Fact]
    public async Task GetAllProviderData_Returns_OK_Result()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v3/providers/all");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProviderDataAsCsv_Returns_OK_Result()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v3/providers/download");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadAsStringAsync();
        result.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetProviderDataCsvFileSize_Returns_OK_Result()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v3/providers/download/size");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var downloadInfo = await response.Content.DeserializeFromHttpContent<ProviderDataDownloadInfoResponse>();
        downloadInfo.Should().NotBeNull();
        downloadInfo.FileSize.Should().Be(4);
    }
}