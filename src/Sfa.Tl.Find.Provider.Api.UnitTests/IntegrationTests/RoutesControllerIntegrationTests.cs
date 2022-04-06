using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.IntegrationTests;

public class RoutesControllerIntegrationTests : IClassFixture<TestServerFactory<FakeStartup>>
{
    private readonly TestServerFactory<FakeStartup> _fixture;

    public RoutesControllerIntegrationTests(TestServerFactory<FakeStartup> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetRoutes_Returns_Ok_Result_For_Valid_Url()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v2/routes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetRoutes_V2_Returns_Ok_Result_For_Valid_Url()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v2/routes/");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}