using Sfa.Tl.Find.Provider.Application.Interfaces;
using System.Net;
using Sfa.Tl.Find.Provider.Application.Models.Authentication;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Models;

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
        if (signInOptions is null) throw new ArgumentNullException(nameof(signInOptions));

        _clientId = signInOptions.Value?.ClientId;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                tokenService.GetApiToken());
    }

    public async Task<(DfeOrganisationInfo OrganisationInfo, DfeUserInfo UserInfo)> GetDfeSignInInfo(string organisationId, string userId)
    {
        var organisationInfoTask = GetOrganisationInfo(organisationId, userId);
        var userInfoTask = GetUserInfo(organisationId, userId);

        await Task.WhenAll(organisationInfoTask, userInfoTask);

        var user = userInfoTask.Result;
        var organisation = organisationInfoTask.Result;

        //if (organisation?.UkPrn == null)
        //{
        //    user.HasAccessToService = false;
        //}

        return (organisation, user);
    }

    private async Task<DfeOrganisationInfo> GetOrganisationInfo(string organisationId, string userId)
    {
        var requestUri = $"/users/{userId}/organisations";

        try
        {
            var response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                var jsonDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                var organisation = jsonDocument
                    .RootElement
                    .EnumerateArray()
                    .Where(e =>
                        string.Compare(e.SafeGetString("id"), organisationId, StringComparison.OrdinalIgnoreCase) == 0)
                    .Select(o => new DfeOrganisationInfo
                    {
                        Id = Guid.Parse(o.SafeGetString("id")),
                        Name = o.SafeGetString("name").ParseTLevelDefinitionName(Constants.QualificationNameMaxLength),
                        UkPrn = long.TryParse(o.SafeGetString("ukprn"), out var ukPrnLong) ? ukPrnLong : null,
                        Urn = long.TryParse(o.SafeGetString("urn"), out var urnLong) ? urnLong : null,
                        Category = int.TryParse(
                            o.GetProperty("category")
                                .SafeGetString("id"), out var category)
                            ? category
                            : 0
                    })
                    .FirstOrDefault();

                return organisation;
            }
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex, "Call to {uri} failed.", requestUri);
        }

        return null;
    }

    private async Task<DfeUserInfo> GetUserInfo(string organisationId, string userId)
    {
        var userClaims = new DfeUserInfo();
        var requestUri = $"/services/{_clientId}/organisations/{organisationId}/users/{userId}";

        try
        {
            var response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
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
                //userClaims.HasAccessToService = false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Call to {uri} failed.", requestUri);
        }

        return userClaims;
    }
}
