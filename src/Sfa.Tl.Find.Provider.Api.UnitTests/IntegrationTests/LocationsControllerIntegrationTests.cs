using System.Net;
using FluentAssertions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.IntegrationTests;

public class LocationsControllerIntegrationTests : IClassFixture<TestServerFactory<FakeStartup>>
{
    private readonly TestServerFactory<FakeStartup> _fixture;

    public LocationsControllerIntegrationTests(TestServerFactory<FakeStartup> fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task GetLocations_Returns_OK_Result_For_Valid_Search_Term()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v3/locations?searchTerm=test");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}