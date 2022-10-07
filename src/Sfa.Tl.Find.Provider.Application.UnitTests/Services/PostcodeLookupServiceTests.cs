using System.Net;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Json;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Application.UnitTests.TestHelpers.Extensions;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Tests.Common.HttpClientHelpers;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Services;

public class PostcodeLookupServiceTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(PostcodeLookupService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(PostcodeLookupService)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task GetPostcode_For_Valid_Postcode_Returns_Expected_Result()
    {
        var validPostcode = GeoLocationBuilder.BuildValidPostcodeLocation();

        var postcodeUriFragment = $"postcodes/{validPostcode.GetUriFormattedPostcode()}";

        var responses = new Dictionary<string, string>
        {
            { postcodeUriFragment, PostcodeLookupJsonBuilder.BuildValidPostcodeResponse() }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.GetPostcode(validPostcode.Location);

        Verify(result, validPostcode);
    }

    [Fact]
    public async Task GetNearestPostcode_Returns_Expected_Result()
    {
        var validPostcode = GeoLocationBuilder.BuildValidPostcodeLocation();

        var postcodeUriFragment = $"postcodes?lon={validPostcode.Longitude}&lat={validPostcode.Latitude}";

        var responses = new Dictionary<string, string>
        {
            { postcodeUriFragment, PostcodeLookupJsonBuilder.BuildNearestPostcodeResponse() }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.GetNearestPostcode(validPostcode.Latitude, validPostcode.Longitude);

        Verify(result, validPostcode);
    }

    [Fact]
    public async Task GetNearestPostcode_Null_Response_Returns_Expected_Result()
    {
        var validPostcode = GeoLocationBuilder.BuildValidPostcodeLocation();

        var postcodeUriFragment = $"postcodes?lon={validPostcode.Longitude}&lat={validPostcode.Latitude}";

        var responses = new Dictionary<string, string>
        {
            { postcodeUriFragment, PostcodeLookupJsonBuilder.BuildNullPostcodeResponse() }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.GetNearestPostcode(validPostcode.Latitude, validPostcode.Longitude);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPostcode_For_Valid_Postcode_Outward_Code_Returns_Expected_Result()
    {
        var validPostcode = GeoLocationBuilder.BuildValidOutwardPostcodeLocation();

        var postcodeUriFragment = $"outcodes/{validPostcode.Location}";

        var responses = new Dictionary<string, string>
        {
            { postcodeUriFragment, PostcodeLookupJsonBuilder.BuildValidOutcodeResponse() }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.GetOutcode(validPostcode.Location);

        Verify(result, validPostcode);
    }

    [Fact]
    public async Task GetPostcode_For_Terminated_Postcode_Returns_Expected_Result()
    {
        var terminatedPostcode = GeoLocationBuilder.BuildTerminatedPostcodeLocation();

        var uriFormattedPostcode = terminatedPostcode.GetUriFormattedPostcode();
        var postcodeUriFragment = $"postcodes/{uriFormattedPostcode}";
        var terminatedPostcodeUriFragment = $"terminated_postcodes/{uriFormattedPostcode}";

        var responses = new Dictionary<string, HttpResponseMessage>
        {
            {
                postcodeUriFragment, FakeResponseFactory.CreateFakeResponse(PostcodeLookupJsonBuilder.BuildPostcodeNotFoundResponse(),
                    responseCode: HttpStatusCode.NotFound)
            },
            {
                terminatedPostcodeUriFragment, FakeResponseFactory.CreateFakeResponse(PostcodeLookupJsonBuilder.BuildTerminatedPostcodeResponse())
            }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.GetPostcode(terminatedPostcode.Location);

        Verify(result, terminatedPostcode);
    }

    [Fact]
    public async Task GetPostcode_For_Valid_Postcode_With_No_Lat_Long_Returns_Expected_Result()
    {
        var noLocationPostcode = GeoLocationBuilder.BuildPostcodeLocationWithDefaultLatLong();

        var postcodeUriFragment = $"postcodes/{noLocationPostcode.GetUriFormattedPostcode()}";

        var responses = new Dictionary<string, string>
        {
            { postcodeUriFragment, PostcodeLookupJsonBuilder.BuildValidPostcodeResponseWithNullLatLong() }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.GetPostcode(noLocationPostcode.Location);

        Verify(result, noLocationPostcode);
    }

    [Fact]
    public async Task GetPostcode_For_Terminated_Postcode_With_No_Lat_Long_Returns_Expected_Result()
    {
        var noLocationTerminatedPostcode = GeoLocationBuilder.BuildTerminatedPostcodeLocationWithDefaultLatLong();

        var uriFormattedPostcode = noLocationTerminatedPostcode.GetUriFormattedPostcode();
        var postcodeUriFragment = $"postcodes/{uriFormattedPostcode}";
        var terminatedPostcodeUriFragment = $"terminated_postcodes/{uriFormattedPostcode}";

        var responses = new Dictionary<string, HttpResponseMessage>
        {
            {
                postcodeUriFragment, FakeResponseFactory.CreateFakeResponse(PostcodeLookupJsonBuilder.BuildPostcodeNotFoundResponse(),
                    responseCode: HttpStatusCode.NotFound)
            },
            {
                terminatedPostcodeUriFragment, FakeResponseFactory.CreateFakeResponse(PostcodeLookupJsonBuilder.BuildValidTerminatedPostcodeResponseWithNullLatLong())
            }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.GetPostcode(noLocationTerminatedPostcode.Location);

        Verify(result, noLocationTerminatedPostcode);

    }

    [Fact]
    public async Task GetPostcode_For_Valid_Postcode_Outward_Code_With_No_Lat_Long_Returns_Expected_Result()
    {
        var noLocationOutcode = GeoLocationBuilder.BuildOutwardPostcodeLocationWithDefaultLatLong();

        var postcodeUriFragment = $"outcodes/{noLocationOutcode.Location}";

        var responses = new Dictionary<string, string>
        {
            { postcodeUriFragment, PostcodeLookupJsonBuilder.BuildValidOutcodeResponseWithNullLatLong() }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.GetOutcode(noLocationOutcode.Location);

        Verify(result, noLocationOutcode);
    }

    [Fact]
    public async Task GetPostcode_For_Invalid_Postcode_Returns_Expected_Result()
    {
        var invalidPostcode = GeoLocationBuilder.BuildInvalidPostcodeLocation();

        var uriFormattedPostcode = invalidPostcode.GetUriFormattedPostcode();
        var postcodeUriFragment = $"postcodes/{uriFormattedPostcode}";
        var terminatedPostcodeUriFragment = $"terminated_postcodes/{uriFormattedPostcode}";

        var responses = new Dictionary<string, HttpResponseMessage>
        {
            {
                postcodeUriFragment, FakeResponseFactory.CreateFakeResponse(PostcodeLookupJsonBuilder.BuildInvalidPostcodeResponse(),
                    responseCode: HttpStatusCode.NotFound)
            },
            {
                terminatedPostcodeUriFragment, FakeResponseFactory.CreateFakeResponse(PostcodeLookupJsonBuilder.BuildInvalidPostcodeResponse(),
                    responseCode: HttpStatusCode.NotFound)
            }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.GetPostcode(invalidPostcode.Location);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPostcode_For_NotFound_Postcode_Returns_Expected_Result()
    {
        var invalidPostcode = GeoLocationBuilder.BuildNotFoundPostcodeLocation();

        var uriFormattedPostcode = invalidPostcode.GetUriFormattedPostcode();
        var postcodeUriFragment = $"postcodes/{uriFormattedPostcode}";
        var terminatedPostcodeUriFragment = $"terminated_postcodes/{uriFormattedPostcode}";

        var responses = new Dictionary<string, HttpResponseMessage>
        {
            {
                postcodeUriFragment, FakeResponseFactory.CreateFakeResponse(PostcodeLookupJsonBuilder.BuildPostcodeNotFoundResponse(),
                    responseCode: HttpStatusCode.NotFound)
            },
            {
                terminatedPostcodeUriFragment, FakeResponseFactory.CreateFakeResponse(PostcodeLookupJsonBuilder.BuildPostcodeNotFoundResponse(),
                    responseCode: HttpStatusCode.NotFound)
            }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.GetPostcode(invalidPostcode.Location);

        result.Should().BeNull();
    }

    private static void Verify(GeoLocation geoLocation,
        GeoLocation expectedGeoLocation)
    {
        geoLocation.Should().NotBeNull();
        geoLocation.Location.Should().Be(expectedGeoLocation.Location);
        geoLocation.Latitude.Should().Be(expectedGeoLocation.Latitude);
        geoLocation.Longitude.Should().Be(expectedGeoLocation.Longitude);
    }
}