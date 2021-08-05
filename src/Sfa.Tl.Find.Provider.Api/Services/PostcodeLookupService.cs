using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Services
{
    public class PostcodeLookupService : IPostcodeLookupService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ILogger<PostcodeLookupService> _logger;

        public PostcodeLookupService(
            IHttpClientFactory httpClientFactory,
            ILogger<PostcodeLookupService> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PostcodeLocation> GetPostcode(string postcode)
        {
            var httpClient = _httpClientFactory.CreateClient(nameof(PostcodeLookupService));

            var responseMessage = await httpClient.GetAsync($"postcodes/{postcode.FormatPostcodeForUri()}");

            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                //Fallback to terminated postcode search
                responseMessage = await httpClient.GetAsync($"terminated_postcodes/{postcode.FormatPostcodeForUri()}");

                if (responseMessage.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }
            }

            var result = ReadPostcodeLocationFromResponse(responseMessage);

            return result.Result;
        }

        private async Task<PostcodeLocation> ReadPostcodeLocationFromResponse(HttpResponseMessage response)
        {
            var jsonDocument = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

            var resultElement = jsonDocument
                .RootElement
                .GetProperty("result");

            return new PostcodeLocation
            {
                Postcode = resultElement.SafeGetString("postcode"),
                Latitude = resultElement.SafeGetDouble("latitude"),
                Longitude = resultElement.SafeGetDouble("longitude")
            };
        }
    }
}
