using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.Services;

public class PostcodeLookupService : IPostcodeLookupService
{
    private readonly HttpClient _httpClient;
    private readonly ICacheService _cacheService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<PostcodeLookupService> _logger;

    public PostcodeLookupService(
        HttpClient httpClient,
        IDateTimeProvider dateTimeProvider,
        ICacheService cacheService,
        ILogger<PostcodeLookupService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<GeoLocation> GetPostcode(string postcode)
    {
        var key = CacheKeys.PostcodeKey(postcode);

        if (!_cacheService.TryGetValue(key, out GeoLocation geoLocation))
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

            Cache(geoLocation, key);
        }

        return geoLocation;
    }

    public async Task<GeoLocation> GetOutcode(string outcode)
    {
        var key = CacheKeys.PostcodeKey(outcode);

        if (!_cacheService.TryGetValue(key, out GeoLocation geoLocation))
        {
            var responseMessage = await _httpClient.GetAsync($"outcodes/{outcode}");

            geoLocation = responseMessage.StatusCode == HttpStatusCode.OK
                ? await ReadPostcodeLocationFromResponse(responseMessage,
                    "outcode")
                : null;

            Cache(geoLocation, key);
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
    private void Cache(GeoLocation geoLocation, string key)
    {
        if (geoLocation is null)
        {
            return;
        }

        _cacheService.Set(key, geoLocation,
            CacheUtilities.DefaultMemoryCacheEntryOptions(
                _dateTimeProvider,
                _logger));
    }
}