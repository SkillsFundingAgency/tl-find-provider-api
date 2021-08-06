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
        private readonly HttpClient _httpClient;

        private readonly ILogger<PostcodeLookupService> _logger;

        public PostcodeLookupService(
            HttpClient httpClient,
            ILogger<PostcodeLookupService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PostcodeLocation> GetPostcode(string postcode)
        {
            var responseMessage = await _httpClient.GetAsync($"postcodes/{postcode.FormatPostcodeForUri()}");

            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                //Fallback to terminated postcode search
                responseMessage = await _httpClient.GetAsync($"terminated_postcodes/{postcode.FormatPostcodeForUri()}");

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
