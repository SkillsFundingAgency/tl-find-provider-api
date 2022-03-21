using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Services;

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

    public async Task<PostcodeLocation> GetOutcode(string outcode)
    {
        var responseMessage = await _httpClient.GetAsync($"outcodes/{outcode}");

        return responseMessage.StatusCode != HttpStatusCode.OK
            ? null
            : await ReadPostcodeLocationFromResponse(responseMessage,
                "outcode");
    }

    public async Task<PostcodeLocation> GetNearestPostcode(double latitude, double longitude)
    {
        var responseMessage = await _httpClient.GetAsync($"postcodes?lon={longitude}&lat={latitude}");

        return responseMessage.StatusCode != HttpStatusCode.OK
            ? null
            : await ReadPostcodeLocationFromResponse(responseMessage);
    }

    private static async Task<PostcodeLocation> ReadPostcodeLocationFromResponse(
        HttpResponseMessage responseMessage,
        string postcodeFieldName = "postcode")
    {
        var jsonDocument = await JsonDocument.ParseAsync(await responseMessage.Content.ReadAsStreamAsync());

        var resultElement = jsonDocument
            .RootElement
            .GetProperty("result");

        switch (resultElement.ValueKind)
        {
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;
            case JsonValueKind.Array:
            {
                var firstItem = resultElement.EnumerateArray().FirstOrDefault();
                {
                    return new PostcodeLocation
                    {
                        Postcode = firstItem.SafeGetString(postcodeFieldName),
                        Latitude = firstItem.SafeGetDouble("latitude", Constants.DefaultLatitude),
                        Longitude = firstItem.SafeGetDouble("longitude")
                    };
                }
            }
            case JsonValueKind.Object:
                return new PostcodeLocation
                {
                    Postcode = resultElement.SafeGetString(postcodeFieldName),
                    Latitude = resultElement.SafeGetDouble("latitude", Constants.DefaultLatitude),
                    Longitude = resultElement.SafeGetDouble("longitude")
                };
            default:
                throw new InvalidOperationException();
        }
    }
}