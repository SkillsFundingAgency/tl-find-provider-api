using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Json;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Services;

public class GoogleMapsApiServiceTests
{
    private const string TestPostcode = "CV1 2WT";
    private const string UnknownPostcode = "OX2 9XX";

    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(GoogleMapsApiService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(GoogleMapsApiService)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task GetAddressDetail_Returns_Empty_Result_For_Empty_Postcode()
    {
        var service = new GoogleMapsApiServiceBuilder()
            .Build();

        var addressDetails = await service.GetAddressDetails("");
        addressDetails.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GetAddressDetail_Returns_Empty_Result_For_Null_Postcode()
    {
        var service = new GoogleMapsApiServiceBuilder()
            .Build();

        var addressDetails = await service.GetAddressDetails(null);
        addressDetails.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GetAddressDetail_Returns_Empty_Result_For_Missing_ApiKey()
    {
        var settings = new SettingsBuilder().BuildGoogleMapsApiSettings(apiKey: "");
        var service = new GoogleMapsApiServiceBuilder()
            .Build(googleMapsApiSettings: settings);
        var addressDetails = await service.GetAddressDetails(TestPostcode);
        addressDetails.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GetAddressDetail_Returns_Expected_Result_For_Valid_Postcode()
    {
        var serviceBuilder = new GoogleMapsApiServiceBuilder();
        var settings = new SettingsBuilder().BuildGoogleMapsApiSettings();

        var responses = new Dictionary<string, string>
        {
            {
                serviceBuilder.BuildUriFragment(TestPostcode, settings.ApiKey),
                GoogleApiJsonBuilder.BuildValidResponse()
            }
        };

        var service = new GoogleMapsApiServiceBuilder()
            .Build(responses, settings);

        var result = await service.GetAddressDetails(TestPostcode);

        result.Should().Be("Coventry");
    }

    [Fact]
    public async Task GetAddressDetail_Returns_Expected_Result_For_Invalid_Postcode()
    {
        var serviceBuilder = new GoogleMapsApiServiceBuilder();
        var settings = new SettingsBuilder().BuildGoogleMapsApiSettings();

        var responses = new Dictionary<string, string>
        {
            {
                serviceBuilder.BuildUriFragment(UnknownPostcode, settings.ApiKey),
                GoogleApiJsonBuilder.BuildZeroResultsResponse()
            }
        };
        var service = new GoogleMapsApiServiceBuilder()
            .Build(responses, settings);

        var result = await service.GetAddressDetails(UnknownPostcode);

        result.Should().BeEmpty();
    }
}