using System.Net;
using FluentAssertions;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Json;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Application.UnitTests.TestHelpers.Extensions;
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
        var validPostcodeLocation = GeoLocationBuilder.BuildValidPostcodeLocation();

        var postcodeUriFragment = $"postcodes/{validPostcodeLocation.GetUriFormattedPostcode()}";

        var responses = new Dictionary<string, string>
        {
            { postcodeUriFragment, PostcodeLookupJsonBuilder.BuildValidPostcodeResponse() }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.GetPostcode(validPostcodeLocation.Location);

        result.Validate(validPostcodeLocation);
    }

    [Fact]
    public async Task GetNearestPostcode_Returns_Expected_Result()
    {
        var validPostcodeLocation = GeoLocationBuilder.BuildValidPostcodeLocation();

        var postcodeUriFragment = $"postcodes?lon={validPostcodeLocation.Longitude}&lat={validPostcodeLocation.Latitude}";

        var responses = new Dictionary<string, string>
        {
            { postcodeUriFragment, PostcodeLookupJsonBuilder.BuildNearestPostcodeResponse() }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.GetNearestPostcode(validPostcodeLocation.Latitude, validPostcodeLocation.Longitude);

        result.Validate(validPostcodeLocation);
    }

    [Fact]
    public async Task GetNearestPostcode_Null_Response_Returns_Expected_Result()
    {
        var validPostcodeLocation = GeoLocationBuilder.BuildValidPostcodeLocation();

        var postcodeUriFragment = $"postcodes?lon={validPostcodeLocation.Longitude}&lat={validPostcodeLocation.Latitude}";

        var responses = new Dictionary<string, string>
        {
            { postcodeUriFragment, PostcodeLookupJsonBuilder.BuildNullPostcodeResponse() }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.GetNearestPostcode(validPostcodeLocation.Latitude, validPostcodeLocation.Longitude);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPostcode_For_Valid_Postcode_Outward_Code_Returns_Expected_Result()
    {
        var validPostcodeLocation = GeoLocationBuilder.BuildValidOutwardPostcodeLocation();

        var postcodeUriFragment = $"outcodes/{validPostcodeLocation.Location}";

        var responses = new Dictionary<string, string>
        {
            { postcodeUriFragment, PostcodeLookupJsonBuilder.BuildValidOutcodeResponse() }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.GetOutcode(validPostcodeLocation.Location);

        result.Validate(validPostcodeLocation);
    }

    [Fact]
    public async Task GetPostcode_For_Terminated_Postcode_Returns_Expected_Result()
    {
        var terminatedPostcodeLocation = GeoLocationBuilder.BuildTerminatedPostcodeLocation();

        var uriFormattedPostcode = terminatedPostcodeLocation.GetUriFormattedPostcode();
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

        var result = await service.GetPostcode(terminatedPostcodeLocation.Location);

        result.Validate(terminatedPostcodeLocation);
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

        result.Validate(noLocationPostcode);
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

        result.Validate(noLocationTerminatedPostcode);

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

        result.Validate(noLocationOutcode);
    }

    [Fact]
    public async Task GetPostcode_For_Invalid_Postcode_Returns_Expected_Result()
    {
        var invalidPostcodeLocation = GeoLocationBuilder.BuildInvalidPostcodeLocation();

        var uriFormattedPostcode = invalidPostcodeLocation.GetUriFormattedPostcode();
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

        var result = await service.GetPostcode(invalidPostcodeLocation.Location);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPostcode_For_NotFound_Postcode_Returns_Expected_Result()
    {
        var invalidPostcodeLocation = GeoLocationBuilder.BuildNotFoundPostcodeLocation();

        var uriFormattedPostcode = invalidPostcodeLocation.GetUriFormattedPostcode();
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

        var result = await service.GetPostcode(invalidPostcodeLocation.Location);

        result.Should().BeNull();
    }
}