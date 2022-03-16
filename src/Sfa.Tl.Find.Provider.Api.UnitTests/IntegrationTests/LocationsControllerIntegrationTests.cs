using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.IntegrationTests;

public class LocationsControllerIntegrationTests : IClassFixture<TestServerFactory<FakeStartup>>
{
    private readonly TestServerFactory<FakeStartup> _fixture;

    public LocationsControllerIntegrationTests(TestServerFactory<FakeStartup> fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task GetProviders_V2_Returns_OK_Result_For_Valid_Postcode_Url()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v2/locations/search?searchString=test");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}