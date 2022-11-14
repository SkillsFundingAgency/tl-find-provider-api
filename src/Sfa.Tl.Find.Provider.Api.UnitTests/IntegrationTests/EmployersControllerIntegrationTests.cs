using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Json;
using System.Net;
using System.Text;
using System.Text.Json;
using Sfa.Tl.Find.Provider.Application.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.IntegrationTests;

public class EmployersControllerIntegrationTests : IClassFixture<TestServerFactory<FakeStartup>>
{
    private readonly TestServerFactory<FakeStartup> _fixture;

    public EmployersControllerIntegrationTests(TestServerFactory<FakeStartup> fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task CreateEmployerInterest_Returns_Ok_Result_For_Valid_Input()
    {
        var testConfigurationSettings = _fixture.GetService<TestConfigurationSettings>();

        var json = PayloadJsonBuilder.BuildCreateEmployerInterestPayload();
        var response = await _fixture
            .CreateClient()
            .PostAsync("/api/v3/employers/createinterest",
                new StringContent(json,
                    Encoding.UTF8,
                    "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseContent);
        var id = jsonDocument
            .RootElement.SafeGetString("id");
        Guid.TryParse(id, out var uniqueId).Should().BeTrue();
        uniqueId.Should().Be(testConfigurationSettings.EmployerInterestUniqueId);
    }

    [Fact]
    public async Task DeleteEmployerInterest_Returns_Ok_Result_For_Valid_Input()
    {
        const string uniqueIdString = "cf6b9dcc-340b-44b2-acf0-d1dbda70d7f1";

        var response = await _fixture
            .CreateClient()
            // ReSharper disable once StringLiteralTypo
            .DeleteAsync($"/api/v3/employers/deleteinterest/{uniqueIdString}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}