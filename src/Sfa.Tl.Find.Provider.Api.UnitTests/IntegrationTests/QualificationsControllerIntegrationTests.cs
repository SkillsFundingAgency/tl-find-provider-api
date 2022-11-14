using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using System.Net;
using Sfa.Tl.Find.Provider.Application.Models;

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
            .GetAsync("/api/v3/qualifications");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var routes = await response.Content.DeserializeFromHttpContent<IList<Qualification>>();
        routes.Should().NotBeNull();
        routes.Count.Should().BeGreaterThan(0);

    }
}