using Sfa.Tl.Find.Provider.Application.Interfaces;
using System.Net;
using Sfa.Tl.Find.Provider.Application.Models.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Sfa.Tl.Find.Provider.Application.Services;
public class DfeSignInApiService : IDfeSignInApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly ILogger<DfeSignInApiService> _logger;

    public DfeSignInApiService(
        HttpClient httpClient,
        IDfeSignInTokenService tokenService,
        IOptions<DfeSignInSettings> signInOptions,
        ILogger<DfeSignInApiService> logger)
    {
        if (tokenService is null) throw new ArgumentNullException(nameof(tokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        if (signInOptions is null) throw new ArgumentNullException(nameof(signInOptions));

        _clientId = signInOptions.Value?.ClientId;

        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                tokenService.GetApiToken());
    }

    public async Task<DfeUserInfo> GetDfeSignInUserInfo(string organisationId, string userId)
    {
        var organisationUkPrn = GetOrganisationUkprn(organisationId, userId);
        var userInfo = GetUserInfo(organisationId, userId);

        await Task.WhenAll(organisationUkPrn, userInfo);

        var userInfoResult = userInfo.Result;
        var ukPrn = organisationUkPrn.Result;

        if (ukPrn.HasValue)
            userInfoResult.UkPrn = ukPrn;
        else
            userInfoResult.HasAccessToService = false;

        return userInfoResult;
    }

    private async Task<long?> GetOrganisationUkprn(string organisationId, string userId)
    {
        var requestUri = $"/users/{userId}/organisations";
        var response = await _httpClient.GetAsync(requestUri);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            var orgToken = Newtonsoft
                    .Json
                    .Linq
                    .JArray
                    .Parse(responseContent)
                    .FirstOrDefault(org =>
                        org.SelectToken("id")
                            .ToString()
                        == organisationId);

            //var o = JsonDocument.Parse(responseContent)
            //        .RootElement
            //TODO: There will be an array here - get the one matching our organisationId
            //        .SafeGetString("id"))
            //    .Where(orgId => orgId == organisationId).ToList();

            return orgToken?["ukprn"].ToObject<long?>();
        }

        return null;
    }

    private async Task<DfeUserInfo> GetUserInfo(string organisationId, string userId)
    {
        var userClaims = new DfeUserInfo();
        var requestUri = $"/services/{_clientId}/organisations/{organisationId}/users/{userId}";
        var response = await _httpClient.GetAsync(requestUri);

        if (response.IsSuccessStatusCode)
        {
            //userClaims = Newtonsoft.Json.JsonConvert
            //    .DeserializeObject<DfeUserInfo>(
            //        await response.Content.ReadAsStringAsync());

            userClaims = JsonSerializer
                .Deserialize<DfeUserInfo>(
                    await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
        }
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {
            userClaims.HasAccessToService = false;
        }

        return userClaims;
    }
}
