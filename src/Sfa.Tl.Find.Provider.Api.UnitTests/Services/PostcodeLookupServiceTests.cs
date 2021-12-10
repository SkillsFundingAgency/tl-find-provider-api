using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.Services;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.HttpClientHelpers;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Services
{
    public class PostcodeLookupServiceTests
    {
        [Fact]
        public void Constructor_Guards_Against_NullParameters()
        {
            typeof(PostcodeLookupService)
                .ShouldNotAcceptNullConstructorArguments();
        }

        [Fact]
        public void Constructor_Guards_Against_BadParameters()
        {
            typeof(PostcodeLookupService)
                .ShouldNotAcceptNullOrBadConstructorArguments();
        }

        [Fact]
        public async Task GetPostcode_For_Valid_Postcode_Returns_Expected_Result()
        {
            var validPostcode = PostcodeLocationBuilder.BuildValidPostcodeLocation();

            var postcodeUriFragment = $"postcodes/{validPostcode.GetUriFormattedPostcode()}";

            var responses = new Dictionary<string, string>
            {
                { postcodeUriFragment, PostcodeLookupJsonBuilder.BuildValidPostcodeResponse() }
            };

            var service = new PostcodeLookupServiceBuilder()
                .Build(responses);

            var result = await service.GetPostcode(validPostcode.Postcode);

            Verify(result, validPostcode);
        }

        [Fact]
        public async Task GetPostcode_For_Valid_Postcode_Outward_Code_Returns_Expected_Result()
        {
            var validPostcode = PostcodeLocationBuilder.BuildValidOutwardPostcodeLocation();

            var postcodeUriFragment = $"outcodes/{validPostcode.Postcode}";

            var responses = new Dictionary<string, string>
            {
                { postcodeUriFragment, PostcodeLookupJsonBuilder.BuildValidOutcodeResponse() }
            };

            var service = new PostcodeLookupServiceBuilder()
                .Build(responses);

            var result = await service.GetOutcode(validPostcode.Postcode);

            Verify(result, validPostcode);
        }

        [Fact]
        public async Task GetPostcode_For_Terminated_Postcode_Returns_Expected_Result()
        {
            var terminatedPostcode = PostcodeLocationBuilder.BuildTerminatedPostcodeLocation();

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

            var result = await service.GetPostcode(terminatedPostcode.Postcode);

            Verify(result, terminatedPostcode);
        }

        [Fact]
        public async Task GetPostcode_For_Valid_Postcode_With_No_Lat_Long_Returns_Expected_Result()
        {
            var noLocationPostcode = PostcodeLocationBuilder.BuildPostcodeLocationWithDefaultLatLong();

            var postcodeUriFragment = $"postcodes/{noLocationPostcode.GetUriFormattedPostcode()}";

            var responses = new Dictionary<string, string>
            {
                { postcodeUriFragment, PostcodeLookupJsonBuilder.BuildValidPostcodeResponseWithNullLatLong() }
            };

            var service = new PostcodeLookupServiceBuilder()
                .Build(responses);

            var result = await service.GetPostcode(noLocationPostcode.Postcode);

            Verify(result, noLocationPostcode);
        }

        [Fact]
        public async Task GetPostcode_For_Terminated_Postcode_With_No_Lat_Long_Returns_Expected_Result()
        {
            var noLocationTerminatedPostcode = PostcodeLocationBuilder.BuildTerminatedPostcodeLocationWithDefaultLatLong();

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

            var result = await service.GetPostcode(noLocationTerminatedPostcode.Postcode);

            Verify(result, noLocationTerminatedPostcode);

        }

        [Fact]
        public async Task GetPostcode_For_Valid_Postcode_Outward_Code_With_No_Lat_Long_Returns_Expected_Result()
        {
            var noLocationOutcode = PostcodeLocationBuilder.BuildOutwardPostcodeLocationWithDefaultLatLong();

            var postcodeUriFragment = $"outcodes/{noLocationOutcode.Postcode}";

            var responses = new Dictionary<string, string>
            {
                { postcodeUriFragment, PostcodeLookupJsonBuilder.BuildValidOutcodeResponseWithNullLatLong() }
            };

            var service = new PostcodeLookupServiceBuilder()
                .Build(responses);

            var result = await service.GetOutcode(noLocationOutcode.Postcode);

            Verify(result, noLocationOutcode);
        }

        [Fact]
        public async Task GetPostcode_For_Invalid_Postcode_Returns_Expected_Result()
        {
            var invalidPostcode = PostcodeLocationBuilder.BuildInvalidPostcodeLocation();

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

            var result = await service.GetPostcode(invalidPostcode.Postcode);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetPostcode_For_NotFound_Postcode_Returns_Expected_Result()
        {
            var invalidPostcode = PostcodeLocationBuilder.BuildNotFoundPostcodeLocation();

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

            var result = await service.GetPostcode(invalidPostcode.Postcode);

            result.Should().BeNull();
        }

        private static void Verify(PostcodeLocation postcodeLocation,
            PostcodeLocation expectedPostcodeLocation)
        {
            postcodeLocation.Should().NotBeNull();
            postcodeLocation.Postcode.Should().Be(expectedPostcodeLocation.Postcode);
            postcodeLocation.Latitude.Should().Be(expectedPostcodeLocation.Latitude);
            postcodeLocation.Longitude.Should().Be(expectedPostcodeLocation.Longitude);
        }
    }
}
