using System.Net;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Tests.Common.HttpClientHelpers;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;

public class GoogleMapsApiServiceBuilder
{
    private const string ApiBaseAbsoluteUri = "https://maps.googleapis.com/maps/api/";
    private static readonly Uri ApiBaseUri = new(ApiBaseAbsoluteUri);

    public IGoogleMapsApiService Build(
        HttpClient httpClient = null,
        GoogleMapsApiSettings googleMapsApiSettings = null,
        ILogger<GoogleMapsApiService> logger = null)
    {
        httpClient ??= Substitute.For<HttpClient>();
        logger ??= Substitute.For<ILogger<GoogleMapsApiService>>();

        googleMapsApiSettings ??= new SettingsBuilder().BuildGoogleMapsApiSettings();
        var googleMapsApiOptions = googleMapsApiSettings.ToOptions();

        return new GoogleMapsApiService(
            httpClient,
            googleMapsApiOptions,
            logger);
    }

    public IGoogleMapsApiService Build(
        IDictionary<string, HttpResponseMessage> responseMessages,
        GoogleMapsApiSettings googleMapsApiSettings = null)
    {
        var apiBaseUri = !string.IsNullOrEmpty(googleMapsApiSettings?.BaseUri)
            ? new Uri(googleMapsApiSettings.BaseUri)
            : ApiBaseUri;

        var responsesWithUri = responseMessages
            .ToDictionary(
                item => new Uri(apiBaseUri, item.Key),
                item => item.Value);

        var httpClient = new TestHttpClientFactory()
            .CreateHttpClientWithBaseUri(ApiBaseUri, responsesWithUri);

        return Build(httpClient, googleMapsApiSettings);
    }

    public IGoogleMapsApiService Build(
        IDictionary<string, string> responseMessages,
        GoogleMapsApiSettings googleMapsApiSettings = null)
    {
        var apiBaseUri = !string.IsNullOrEmpty(googleMapsApiSettings?.BaseUri)
            ? new Uri(googleMapsApiSettings.BaseUri)
            : ApiBaseUri;

        var responsesWithUri = responseMessages
            .ToDictionary(
                item => new Uri(apiBaseUri, item.Key),
                item => item.Value);

        var httpClient = new TestHttpClientFactory()
            .CreateHttpClientWithBaseUri(ApiBaseUri, responsesWithUri);

        return Build(httpClient, googleMapsApiSettings);
    }

    // ReSharper disable once StringLiteralTypo
    public string BuildUriFragment(string postcode, string apiKey) =>
        $"place/textsearch/json?region=uk&radius=1&key={apiKey}&query={WebUtility.UrlEncode(postcode)}";
}