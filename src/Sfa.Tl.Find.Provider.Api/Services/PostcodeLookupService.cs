using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Services
{
    public class PostcodeLookupService : IPostcodeLookupService
    {
        private readonly HttpClient _httpClient;

        public PostcodeLookupService(
            HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
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

            return await ReadPostcodeLocationFromResponse(responseMessage);
        }

        private static async Task<PostcodeLocation> ReadPostcodeLocationFromResponse(HttpResponseMessage responseMessage)
        {
            var jsonDocument = await JsonDocument.ParseAsync(await responseMessage.Content.ReadAsStreamAsync());

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
