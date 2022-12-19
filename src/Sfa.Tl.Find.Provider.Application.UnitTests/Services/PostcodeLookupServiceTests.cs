using System.Net;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Json;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Application.UnitTests.TestHelpers.Extensions;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Tests.Common.HttpClientHelpers;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

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
    public async Task GetPostcode_For_Valid_Outcode_Returns_Expected_Result()
    {
        var validOutcodeLocation = GeoLocationBuilder.BuildValidOutcodeLocation();

        var outcodeUriFragment = $"outcodes/{validOutcodeLocation.Location}";

        var responses = new Dictionary<string, string>
        {
            { outcodeUriFragment, PostcodeLookupJsonBuilder.BuildValidOutcodeResponse() }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.GetOutcode(validOutcodeLocation.Location);

        result.Validate(validOutcodeLocation);
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
    public async Task GetPostcode_For_Valid_Outcode_With_No_Lat_Long_Returns_Expected_Result()
    {
        var noLocationOutcode = GeoLocationBuilder.BuildOutcodeLocationWithDefaultLatLong();

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

    [Fact]
    public async Task IsValid_Returns_True_For_Valid_Postcode()
    {
        var validPostcodeLocation = GeoLocationBuilder.BuildValidPostcodeLocation();

        var postcodeUriFragment = $"postcodes/{validPostcodeLocation.GetUriFormattedPostcode()}";

        var responses = new Dictionary<string, string>
        {
            { postcodeUriFragment, PostcodeLookupJsonBuilder.BuildValidPostcodeResponse() }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.IsValid(validPostcodeLocation.Location);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsValid_Returns_False_For_Invalid_Postcode()
    {
        var invalidPostcodeLocation = GeoLocationBuilder.BuildInvalidPostcodeLocation();

        var postcodeUriFragment = $"postcodes/{invalidPostcodeLocation.GetUriFormattedPostcode()}/validate";

        var responses = new Dictionary<string, string>
        {
            { postcodeUriFragment, PostcodeLookupJsonBuilder.BuildPostcodeValidationFailResponse() }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.IsValid(invalidPostcodeLocation.Location);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsValidOutcode_Returns_True_For_Valid_Outcode()
    {
        var validOutcodeLocation = GeoLocationBuilder.BuildValidOutcodeLocation();
        var outcodeUriFragment = $"outcodes/{validOutcodeLocation.Location}";

        var responses = new Dictionary<string, HttpResponseMessage>
        {
            {
                outcodeUriFragment, FakeResponseFactory.CreateFakeResponse(PostcodeLookupJsonBuilder.BuildValidOutcodeResponse())
            }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.IsValidOutcode(validOutcodeLocation.Location);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsValidOutCode_Returns_True_For_Invalid_Outcode()
    {
        var invalidOutcodeLocation = GeoLocationBuilder.BuildInvalidOutcodeLocation();

        var outcodeUriFragment = $"outcodes/{invalidOutcodeLocation.Location}";

        var responses = new Dictionary<string, HttpResponseMessage>
        {
            {
                outcodeUriFragment, FakeResponseFactory.CreateFakeResponse(PostcodeLookupJsonBuilder.BuildOutcodeNotFoundResponse(),
                    responseCode: HttpStatusCode.NotFound)
            }
        };

        var service = new PostcodeLookupServiceBuilder()
            .Build(responses);

        var result = await service.IsValidOutcode(invalidOutcodeLocation.Location);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetPostcode_For_Valid_Postcode_Returns_Expected_ResultFrom_Cache()
    {
        var validPostcodeLocation = GeoLocationBuilder.BuildValidPostcodeLocation();

        var cacheService = Substitute.For<ICacheService>();
        cacheService.Get<GeoLocation?>(Arg.Is<string>(x =>
                x.Contains(validPostcodeLocation.Location.Replace(" ", ""))))
            .Returns(validPostcodeLocation);

        var service = new PostcodeLookupServiceBuilder()
            .Build(cacheService: cacheService);

        var result = await service.GetPostcode(validPostcodeLocation.Location);

        result.Validate(validPostcodeLocation);
    }

    [Fact]
    public async Task GetOutcode_For_Valid_Outcode_Returns_Expected_ResultFrom_Cache()
    {
        var validOutcodeLocation = GeoLocationBuilder.BuildValidOutcodeLocation();

        var cacheService = Substitute.For<ICacheService>();
        cacheService.Get<GeoLocation?>(Arg.Is<string>(x =>
                x.Contains(validOutcodeLocation.Location.Replace(" ", ""))))
            .Returns(validOutcodeLocation);

        var service = new PostcodeLookupServiceBuilder()
            .Build(cacheService: cacheService);

        var result = await service.GetOutcode(validOutcodeLocation.Location);

        result.Validate(validOutcodeLocation);
    }
}