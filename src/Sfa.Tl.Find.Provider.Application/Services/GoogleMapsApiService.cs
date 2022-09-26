using Sfa.Tl.Find.Provider.Application.Interfaces;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Extensions;

namespace Sfa.Tl.Find.Provider.Application.Services;
public class GoogleMapsApiService : IGoogleMapsApiService
{
    private readonly HttpClient _httpClient;
    private readonly GoogleMapsApiSettings _googleMapsApiSettings;
    private readonly ILogger<GoogleMapsApiService> _logger;

    public GoogleMapsApiService(
        HttpClient httpClient,
        IOptions<GoogleMapsApiSettings> googleMapsApiOptions,
        ILogger<GoogleMapsApiService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _googleMapsApiSettings = googleMapsApiOptions?.Value
                                 ?? throw new ArgumentNullException(nameof(googleMapsApiOptions));
    }

    public async Task<string> GetAddressDetails(string postcode)
    {
        if (string.IsNullOrWhiteSpace(_googleMapsApiSettings.ApiKey))
        {
            _logger.LogWarning("{serviceName} called with missing api key. No results could be returned.", nameof(GoogleMapsApiService));
            return default;
        }
        if (string.IsNullOrWhiteSpace(postcode)) return default;

        // ReSharper disable once StringLiteralTypo
        var lookupUrl = $"place/textsearch/json?region=uk&radius=1&key={_googleMapsApiSettings.ApiKey}&query={WebUtility.UrlEncode(postcode.Trim())}";

        var responseMessage = await _httpClient.GetAsync(lookupUrl);

        responseMessage.EnsureSuccessStatusCode();

        var jsonDocument = await JsonDocument.ParseAsync(await responseMessage.Content.ReadAsStreamAsync());

        var documentRoot = jsonDocument
            .RootElement;

        var town = default(string);
        var status = documentRoot.SafeGetString("status");

        if (status == "OK")
        {
            town = documentRoot
                .GetProperty("results")
                .EnumerateArray()
                .First()
                .GetProperty("formatted_address")
                .GetString()
                ?.Split(",")
                .Last()
                .Replace(postcode.Replace(" ", ""), string.Empty)
                .Trim();
        }

        return town ?? string.Empty;
    }
}
