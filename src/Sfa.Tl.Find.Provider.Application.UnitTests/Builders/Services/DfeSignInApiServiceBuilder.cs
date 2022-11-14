using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Tests.Common.HttpClientHelpers;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
public class DfeSignInApiServiceBuilder
{
    public IDfeSignInApiService Build(
        HttpClient httpClient = null,
        IDfeSignInTokenService tokenService = null,
        DfeSignInSettings signInSettings = null,
        ILogger<DfeSignInApiService> logger = null)
    {
        httpClient ??= Substitute.For<HttpClient>();
        logger ??= Substitute.For<ILogger<DfeSignInApiService>>();

        tokenService ??= new DfeSignInTokenServiceBuilder()
            .Build();

        signInSettings ??= new SettingsBuilder().BuildDfeSignInSettings();
        var signInOptions = signInSettings.ToOptions();

        return new DfeSignInApiService(
            httpClient,
            tokenService,
            signInOptions,
            logger);
    }

    public IDfeSignInApiService Build(
        IDictionary<string, string> responseMessages,
        DfeSignInSettings dfeSignInSettings)
    {
        var apiBaseUri = new Uri(dfeSignInSettings.ApiUri.TrimEnd('/'));

        var responsesWithUri = responseMessages
            .ToDictionary(
                item => new Uri(apiBaseUri, item.Key),
                item => item.Value);

        var httpClient = new TestHttpClientFactory()
            .CreateHttpClientWithBaseUri(apiBaseUri, responsesWithUri);

        return Build(
            httpClient, 
            signInSettings: dfeSignInSettings);
    }

    public string BuildGetUserUriFragment(
        string organisationId,
        string userId,
        DfeSignInSettings dfeSignInSettings) =>
        $"/services/{dfeSignInSettings.ClientId}/organisations/{organisationId}/users/{userId}";

    public string BuildGetOrganisationsUriFragment(string userId) =>
        $"/users/{userId}/organisations";
}
