using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.IntegrationTests;

public class QualificationsControllerIntegrationTests : IClassFixture<TestServerFactory<FakeStartup>>
{
    private readonly TestServerFactory<FakeStartup> _fixture;

    public QualificationsControllerIntegrationTests(TestServerFactory<FakeStartup> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetQualifications_Returns_OK_Result_For_Valid_Url()
    {
        var response = await _fixture
            .CreateClient()
            .GetAsync("/api/v2/qualifications");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}