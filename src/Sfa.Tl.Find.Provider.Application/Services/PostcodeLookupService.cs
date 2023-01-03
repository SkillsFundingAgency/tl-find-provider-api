using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.Services;

public class PostcodeLookupService : IPostcodeLookupService
{
    private readonly HttpClient _httpClient;
    private readonly ICacheService _cacheService;

    public PostcodeLookupService(
        HttpClient httpClient,
        ICacheService cacheService)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    public async Task<GeoLocation> GetPostcode(string postcode)
    {
        var key = CacheKeys.PostcodeKey(postcode);

        var geoLocation = await _cacheService.Get<GeoLocation?>(key);
        if (geoLocation is null)
        {
            var responseMessage = await _httpClient.GetAsync($"postcodes/{postcode.FormatPostcodeForUri()}");

            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                //Fallback to terminated postcode search
                responseMessage = await _httpClient.GetAsync($"terminated_postcodes/{postcode.FormatPostcodeForUri()}");
            }

            geoLocation = responseMessage.StatusCode == HttpStatusCode.OK
                ? await ReadPostcodeLocationFromResponse(responseMessage)
                : null;

            await Cache(geoLocation, key);
        }

        return geoLocation;
    }

    public async Task<GeoLocation> GetOutcode(string outcode)
    {
        var key = CacheKeys.PostcodeKey(outcode);

        var geoLocation = await _cacheService.Get<GeoLocation?>(key);
        if (geoLocation is null)
        {
            var responseMessage = await _httpClient.GetAsync($"outcodes/{outcode}");

            geoLocation = responseMessage.StatusCode == HttpStatusCode.OK
                ? await ReadPostcodeLocationFromResponse(responseMessage,
                    "outcode")
                : null;

            await Cache(geoLocation, key);
        }

        return geoLocation;
    }

    public async Task<GeoLocation> GetNearestPostcode(double latitude, double longitude)
    {
        var responseMessage = await _httpClient.GetAsync($"postcodes?lon={longitude}&lat={latitude}");

        return responseMessage.StatusCode == HttpStatusCode.OK
            ? await ReadPostcodeLocationFromResponse(responseMessage)
            : null;
    }

    public async Task<bool> IsValid(string postcode) =>
        await GetPostcode(postcode) is not null;

    public async Task<bool> IsValidOutcode(string outcode) =>
        await GetOutcode(outcode) is not null;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task Cache(GeoLocation geoLocation, string key)
    {
        if (geoLocation is null)
        {
            return;
        }

        await _cacheService.Set(key, geoLocation, CacheDuration.Medium);
    }
}