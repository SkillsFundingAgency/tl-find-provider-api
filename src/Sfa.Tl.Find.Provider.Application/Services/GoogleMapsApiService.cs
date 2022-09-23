using Sfa.Tl.Find.Provider.Application.Interfaces;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using System.Net;
using System.Text.Json;

namespace Sfa.Tl.Find.Provider.Application.Services;
public class GoogleMapsApiService : IGoogleMapsApiService
{
    private readonly HttpClient _httpClient;
    private readonly GoogleMapsApiSettings _googleMapsApiSettings;

    public GoogleMapsApiService(
        HttpClient httpClient,
        IOptions<GoogleMapsApiSettings> googleMapsApiOptions)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _googleMapsApiSettings = googleMapsApiOptions?.Value
                                 ?? throw new ArgumentNullException(nameof(googleMapsApiOptions));
    }

    public async Task<string> GetAddressDetails(string postcode)
    {
        if (string.IsNullOrWhiteSpace(_googleMapsApiSettings.ApiKey)) return null;

        // ReSharper disable once StringLiteralTypo
        var lookupUrl = $"place/textsearch/json?region=uk&radius=1&key={_googleMapsApiSettings.ApiKey}&query={WebUtility.UrlEncode(postcode.Trim())}";

        var responseMessage = await _httpClient.GetAsync(lookupUrl);

        responseMessage.EnsureSuccessStatusCode();

        var jsonDocument = await JsonDocument.ParseAsync(await responseMessage.Content.ReadAsStreamAsync());

        var documentRoot = jsonDocument
            .RootElement;

        var status =
            documentRoot
                .GetProperty("status")
                .GetString();

        if (status != "OK")
            return string.Empty;

        var town =
            documentRoot
                .GetProperty("results")
                .EnumerateArray()
                .First()
                .GetProperty("formatted_address")
                .GetString()
                ?.Split(",")
                .Last()
                .Replace(postcode, string.Empty)
                .Trim();

        return town ?? string.Empty;
    }
}
