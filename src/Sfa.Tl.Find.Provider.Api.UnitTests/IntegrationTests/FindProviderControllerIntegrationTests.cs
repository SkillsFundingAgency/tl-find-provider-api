using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.IntegrationTests;

public class FindProviderControllerIntegrationTests : IClassFixture<TestServerFactory<FakeStartup>>
{
    private readonly TestServerFactory<FakeStartup> _fixture;

    public FindProviderControllerIntegrationTests(TestServerFactory<FakeStartup> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetProviders_Returns_OK_Result_For_Valid_Url()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v1/findproviders/providers?postcode=CV1+2WT&qualificationId=40&routeId=5&page=0&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProviders_V2_Returns_OK_Result_For_Valid_Postcode_Url()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v2/findproviders/providers?postcode=CV1+2WT&page=0&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProviders_V2_Returns_OK_Result_For_Valid_Lat_Long_Url()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v2/findproviders/providers?lat=52.400997&lon=-1.508122&page=0&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetProviders_Returns_OK_Result_For_Short_Postcode_Url()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v1/findproviders/providers?postcode=L1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var searchResponse = await response.Content.DeserializeFromHttpContent();
        searchResponse.Should().NotBeNull();
        searchResponse!.Error.Should().BeNull();
    }

    [Fact]
    public async Task GetProviders_Returns_Error_Message_Result_For_Missing_Postcode()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v1/findproviders/providers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var searchResponse = await response.Content.DeserializeFromHttpContent();
        searchResponse.Should().NotBeNull();
        searchResponse!.Error.Should().Be("The postcode field is required.");
    }

    [Fact]
    public async Task GetProviders_Returns_Error_Message_Result_For_Postcode_Too_Long()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v1/findproviders/providers?postcode=ABC+DEF+GHI");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var searchResponse = await response.Content.DeserializeFromHttpContent();
        searchResponse.Should().NotBeNull();
        searchResponse!.Error.Should().Be("The postcode field must be no more than 8 characters.");
    }

    [Fact]
    public async Task GetProviders_Returns_Error_Message_Result_For_Postcode_Too_Short()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v1/findproviders/providers?postcode=A");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var searchResponse = await response.Content.DeserializeFromHttpContent();
        searchResponse.Should().NotBeNull();
        searchResponse!.Error.Should().Be("The postcode field must be at least 2 characters.");
    }

    [Fact]
    public async Task GetProviders_Returns_Bad_Request_Result_For_Zero_PageSize()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v1/findproviders/providers?postcode=CV1+2WT&page=0&pageSize=0");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await response.Content.ValidateProblemDetails(
            ("pageSize", "The pageSize field must be at least one."));
    }

    [Fact]
    public async Task GetProviders_Returns_Error_Message_Result_For_Postcode_With_Illegal_Characters()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v1/findproviders/providers?postcode=CV1+2WT£");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var searchResponse = await response.Content.DeserializeFromHttpContent();
        searchResponse.Should().NotBeNull();
        searchResponse!.Error.Should().Be("The postcode field must start with a letter and contain only letters, numbers, and an optional space.");
    }
    
    [Fact]
    public async Task GetProviders_Returns_Error_Message_Result_For_Postcode_Starting_With_Number()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v1/findproviders/providers?postcode=2V1+2WT");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var searchResponse = await response.Content.DeserializeFromHttpContent();
        searchResponse.Should().NotBeNull();
        searchResponse!.Error.Should().Be("The postcode field must start with a letter and contain only letters, numbers, and an optional space.");
    }

    [Fact]
    public async Task GetProviders_Returns_Bad_Request_Result_For_Negative_Page()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v1/findproviders/providers?postcode=CV1+2WT&page=-1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await response.Content.ValidateProblemDetails(
            ("page", "The page field must be zero or greater."));
    }

    [Fact]
    public async Task GetProviders_Returns_Bad_Request_Result_With_All_Errors()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v1/findproviders/providers?qualificationId=40&page=-1&pageSize=0");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await response.Content.ValidateProblemDetails(
            ("pageSize", "The pageSize field must be at least one."),
            ("page", "The page field must be zero or greater."));
    }

    [Fact]
    public async Task GetQualifications_Returns_OK_Result_For_Valid_Url()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v1/findproviders/qualifications");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetQualifications_V2_Returns_OK_Result_For_Valid_Url()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v2/findproviders/qualifications");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetRoutes_Returns_Ok_Result_For_Valid_Url()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v1/findproviders/routes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetRoutes_V2_Returns_Ok_Result_For_Valid_Url()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v2/findproviders/routes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}