using Sfa.Tl.Find.Provider.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
    private readonly DfeSignInSettings _signInSettings;
    private readonly string _clientId;
    private readonly IDfeSignInTokenService _tokenService;
    private readonly ILogger<DfeSignInApiService> _logger;

    public DfeSignInApiService(
        HttpClient httpClient,
        IDfeSignInTokenService tokenService,
        IOptions<DfeSignInSettings> signInOptions,
        ILogger<DfeSignInApiService> logger)
    {
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        if (signInOptions is null) throw new ArgumentNullException(nameof(signInOptions));

        _signInSettings = signInOptions.Value;
        _clientId = _signInSettings?.ClientId;

        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _tokenService.GetApiToken());
    }

    public async Task<DfeUserInfo> GetDfeSignInUserInfo(string organisationId, string userId)
    {
        var users = await GetUserList();
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

            //TODO: Change to System.Text.Json
            var orgToken = Newtonsoft.Json.Linq.JArray.Parse(responseContent).FirstOrDefault(org => org.SelectToken("id").ToString() == organisationId);
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
            var responseContent = await response.Content.ReadAsStringAsync();
            userClaims = Newtonsoft.Json.JsonConvert
                .DeserializeObject<DfeUserInfo>(responseContent);

            //TODO: Use this...
            var userClaims2 = JsonSerializer
                .Deserialize<DfeUserInfo>(
                    responseContent
                //await content.ReadAsStringAsync(),
                //new JsonSerializerOptions
                //{
                //   AllowTrailingCommas = true
                //}
                );
        }
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {
            userClaims.HasAccessToService = false;
        }

        return userClaims;
    }

    //TODO: Remove this, it's for testing only
    private async Task<IList<string>> GetUserList()
    {
        var users = new List<string>();
        var requestUri = "users?page=1&pageSize=25";
        var response = await _httpClient.GetAsync(requestUri);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
        }

        return users;
    }

}
