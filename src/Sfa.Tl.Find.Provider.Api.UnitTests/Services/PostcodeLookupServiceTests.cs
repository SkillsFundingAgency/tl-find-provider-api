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
            var validPostcode = new PostcodeLocationBuilder().BuildValidPostcodeLocation();

            var postcodeUriFragment = $"postcodes/{validPostcode.Postcode.Replace(" ", "%20")}";

            var jsonBuilder = new PostcodeLookupJsonBuilder();

            var responses = new Dictionary<string, string>
            {
                { postcodeUriFragment, jsonBuilder.BuildValidPostcodeResponse() }
            };

            var service = new PostcodeLookupServiceBuilder()
                .Build(responses);

            var result = await service.GetPostcode(validPostcode.Postcode);
            
            Verify(result, validPostcode);
        }

        [Fact]
        public async Task GetPostcode_For_Terminated_Postcode_Returns_Expected_Result()
        {
            var terminatedPostcode = new PostcodeLocationBuilder().BuildTerminatedPostcodeLocation();

            var uriFormattedPostcode = terminatedPostcode.Postcode.Replace(" ", "%20");
            var postcodeUriFragment = $"postcodes/{uriFormattedPostcode}";
            var terminatedPostcodeUriFragment = $"terminated_postcodes/{uriFormattedPostcode}";

            var jsonBuilder = new PostcodeLookupJsonBuilder();
            
            var responses = new Dictionary<string, HttpResponseMessage>
            {
                {
                    postcodeUriFragment, FakeResponseFactory.CreateFakeResponse(jsonBuilder.BuildPostcodeNotFoundResponse(),
                        responseCode: HttpStatusCode.NotFound)
                },
                {
                    terminatedPostcodeUriFragment, FakeResponseFactory.CreateFakeResponse(jsonBuilder.BuildTerminatedPostcodeResponse())
                }
            };

            var service = new PostcodeLookupServiceBuilder()
                .Build(responses);

            var result = await service.GetPostcode(terminatedPostcode.Postcode);

            Verify(result, terminatedPostcode);
        }

        [Fact]
        public async Task GetPostcode_For_Invalid_Postcode_Returns_Expected_Result()
        {
            var invalidPostcode = new PostcodeLocationBuilder().BuildInvalidPostcodeLocation();

            var uriFormattedPostcode = invalidPostcode.Postcode.Replace(" ", "%20");
            var postcodeUriFragment = $"postcodes/{uriFormattedPostcode}";
            var terminatedPostcodeUriFragment = $"terminated_postcodes/{uriFormattedPostcode}";

            var jsonBuilder = new PostcodeLookupJsonBuilder();

            var responses = new Dictionary<string, HttpResponseMessage>
            {
                {
                    postcodeUriFragment, FakeResponseFactory.CreateFakeResponse(jsonBuilder.BuildInvalidPostcodeResponse(),
                        responseCode: HttpStatusCode.NotFound)
                },
                {
                    terminatedPostcodeUriFragment, FakeResponseFactory.CreateFakeResponse(jsonBuilder.BuildInvalidPostcodeResponse(),
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
            var invalidPostcode = new PostcodeLocationBuilder().BuildNotFoundPostcodeLocation();

            var uriFormattedPostcode = invalidPostcode.Postcode.Replace(" ", "%20");
            var postcodeUriFragment = $"postcodes/{uriFormattedPostcode}";
            var terminatedPostcodeUriFragment = $"terminated_postcodes/{uriFormattedPostcode}";

            var jsonBuilder = new PostcodeLookupJsonBuilder();

            var responses = new Dictionary<string, HttpResponseMessage>
            {
                {
                    postcodeUriFragment, FakeResponseFactory.CreateFakeResponse(jsonBuilder.BuildPostcodeNotFoundResponse(),
                        responseCode: HttpStatusCode.NotFound)
                },
                {
                    terminatedPostcodeUriFragment, FakeResponseFactory.CreateFakeResponse(jsonBuilder.BuildPostcodeNotFoundResponse(),
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
