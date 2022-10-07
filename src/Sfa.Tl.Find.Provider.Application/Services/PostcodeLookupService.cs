using System.Net;
using System.Text.Json;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Services;

public class PostcodeLookupService : IPostcodeLookupService
{
    private readonly HttpClient _httpClient;

    public PostcodeLookupService(
        HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<GeoLocation> GetPostcode(string postcode)
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

    public async Task<GeoLocation> GetOutcode(string outcode)
    {
        var responseMessage = await _httpClient.GetAsync($"outcodes/{outcode}");

        return responseMessage.StatusCode != HttpStatusCode.OK
            ? null
            : await ReadPostcodeLocationFromResponse(responseMessage,
                "outcode");
    }

    public async Task<GeoLocation> GetNearestPostcode(double latitude, double longitude)
    {
        var responseMessage = await _httpClient.GetAsync($"postcodes?lon={longitude}&lat={latitude}");

        return responseMessage.StatusCode != HttpStatusCode.OK
            ? null
            : await ReadPostcodeLocationFromResponse(responseMessage);
    }

    public async Task<bool> IsValid(string postcode)
    {
        var responseMessage = await _httpClient.GetAsync($"postcodes/{postcode.FormatPostcodeForUri()}/validate");

        if (responseMessage.IsSuccessStatusCode)
        {
            (await JsonDocument.ParseAsync(await responseMessage.Content.ReadAsStreamAsync()))
            //var resultElement = jsonDocument
                .RootElement
                .TryGetProperty("result", out var result);

            if (result.ValueKind is JsonValueKind.True)
            {
                return true;
            }
        }
        return false;
    }

    private static async Task<GeoLocation> ReadPostcodeLocationFromResponse(
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
                        return new GeoLocation
                        {
                            Location = firstItem.SafeGetString(postcodeFieldName),
                            Latitude = firstItem.SafeGetDouble("latitude", Constants.DefaultLatitude),
                            Longitude = firstItem.SafeGetDouble("longitude")
                        };
                    }
                }
            case JsonValueKind.Object:
                return new GeoLocation
                {
                    Location = resultElement.SafeGetString(postcodeFieldName),
                    Latitude = resultElement.SafeGetDouble("latitude", Constants.DefaultLatitude),
                    Longitude = resultElement.SafeGetDouble("longitude")
                };
            default:
                throw new InvalidOperationException();
        }
    }
}