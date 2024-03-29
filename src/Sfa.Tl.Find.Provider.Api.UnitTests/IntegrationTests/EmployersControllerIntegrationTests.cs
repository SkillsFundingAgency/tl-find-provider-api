﻿using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Json;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using System.Net;
using System.Text;
using System.Text.Json;

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

        PayloadJsonBuilder.BuildCreateEmployerInterestPayload();
        var response = await _fixture
            .CreateClient()
            .PostAsync("/api/v3/employers/createinterest",
                new StringContent(
                    PayloadJsonBuilder
                        .BuildCreateEmployerInterestPayload(),
                    Encoding.UTF8,
                    "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseContent);

        var idList = jsonDocument
            .RootElement
            .GetProperty("ids")
            .EnumerateArray()
            .Select(p => 
                p.GetGuid())
            .ToList();

        idList.Count.Should().Be(1);
        idList.First().Should().Be(testConfigurationSettings.EmployerInterestUniqueId);
    }

    [Fact]
    public async Task CreateEmployerInterest_Returns_Ok_Result_For_Multiple_Locations()
    {
        var testConfigurationSettings = _fixture.GetService<TestConfigurationSettings>();

        var response = await _fixture
            .CreateClient()
            .PostAsync("/api/v3/employers/createinterest",
                new StringContent(
                    PayloadJsonBuilder
                        .BuildCreateEmployerInterestPayloadWithTwoLocations(),
                    Encoding.UTF8,
                    "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseContent);

        var idList = jsonDocument
            .RootElement
            .GetProperty("ids")
            .EnumerateArray()
            .Select(p =>
                p.GetGuid())
            .ToList();

        idList.Count.Should().Be(2);
        //Values will be identical because the fake startup only knows one guid
        idList[0].Should().Be(testConfigurationSettings.EmployerInterestUniqueId);
        idList[1].Should().Be(testConfigurationSettings.EmployerInterestUniqueId);
    }

    [Fact]
    public async Task CreateEmployerInterest_Returns_Bad_Request_Result_For_Missing_Locations()
    {
        PayloadJsonBuilder.BuildCreateEmployerInterestPayloadWithNoLocations();
        var response = await _fixture
            .CreateClient()
            .PostAsync("/api/v3/employers/createinterest",
                new StringContent(
                    PayloadJsonBuilder
                        .BuildCreateEmployerInterestPayloadWithNoLocations(),
                    Encoding.UTF8,
                    "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await response.Content.ValidateProblemDetails(
            ("Locations", "One or more locations must be provided."));
    }

    [Fact]
    public async Task CreateEmployerInterest_Returns_Bad_Request_Result_For_Empty_Locations()
    {
        PayloadJsonBuilder.BuildCreateEmployerInterestPayloadWithEmptyLocations();
        var response = await _fixture
            .CreateClient()
            .PostAsync("/api/v3/employers/createinterest",
                new StringContent(
                    PayloadJsonBuilder
                        .BuildCreateEmployerInterestPayloadWithNoLocations(),
                    Encoding.UTF8,
                    "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await response.Content.ValidateProblemDetails(
            ("Locations", "One or more locations must be provided."));
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