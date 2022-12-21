using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Filters;

public class HmacAuthorizationFilter : IAsyncAuthorizationFilter
{
    private static readonly ConcurrentDictionary<string, string> AllowedApps = new();

    private const string AuthenticationScheme = "amx";
    private const ulong RequestMaxAgeInSeconds = 300;

    private readonly ICacheService _cacheService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<HmacAuthorizationFilter> _logger;

    public HmacAuthorizationFilter(
        IOptions<ApiSettings> apiOptions,
        ICacheService cacheService,
        IDateTimeProvider dateTimeProvider,
        ILogger<HmacAuthorizationFilter> logger)
    {
        if (apiOptions is null) throw new ArgumentNullException(nameof(apiOptions));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (AllowedApps.IsEmpty)
        {
            AllowedApps.TryAdd(apiOptions.Value.AppId, apiOptions.Value.ApiKey);
        }
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        try
        {
            if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader)
                && AuthenticationHeaderValue.TryParse(authHeader, out var auth)
                && auth.Scheme == AuthenticationScheme)
            {
                var credentials = GetAuthorizationHeaderValues(auth.Parameter);

                if (credentials != null)
                {
                    var isValid = await IsValidRequest(context.HttpContext.Request,
                        credentials.Value.appId,
                        credentials.Value.incomingBase64Signature,
                        credentials.Value.nonce,
                        credentials.Value.requestTimestamp,
                        credentials.Value.skipBodyEncoding);

                    if (isValid)
                    {
                        return;
                    }

                    context.Result = new UnauthorizedObjectResult("Incorrect 'Authorization' header.");
                }
            }

            context.Result ??= new UnauthorizedObjectResult("Missing or malformed 'Authorization' header.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in {nameof(HmacAuthorizationFilter)}.");
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    private async Task<bool> IsValidRequest(HttpRequest request, string appId, string incomingBase64Signature, string nonce, string requestTimestamp, bool skipBodyEncoding)
    {
        if (!AllowedApps.ContainsKey(appId))
        {
            _logger.LogWarning("Failed authentication - application id is not allowed.");
            return false;
        }

        if (await IsReplayRequest(nonce, requestTimestamp))
        {
            return false;
        }

        var requestContentBase64String = "";

        if (!skipBodyEncoding)
        {
            var hash = await ComputeHash(request);
            if (hash != null)
            {
                requestContentBase64String = Convert.ToBase64String(hash);
            }
        }

        var requestUri = request.GetDisplayUrl().ToLower();
        var sharedKey = AllowedApps[appId];

        var data = $"{appId}{request.Method.ToUpper()}{requestUri}{requestTimestamp}{nonce}{requestContentBase64String}";
        var secretKeyBytes = Encoding.ASCII.GetBytes(sharedKey);
        var signature = Encoding.ASCII.GetBytes(data);
        using var hmac = new HMACSHA256(secretKeyBytes);
        var signatureBytes = hmac.ComputeHash(signature);
        var base64Signature = Convert.ToBase64String(signatureBytes);

        return incomingBase64Signature.Equals(base64Signature, StringComparison.Ordinal);
    }

    private async Task<bool> IsReplayRequest(string nonce, string requestTimestamp)
    {
        if (await _cacheService.KeyExists<string>(nonce))
        {
            _logger.LogWarning("Replay request detected - nonce found in cache.");
            return true;
        }

        var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
        var currentTs = _dateTimeProvider.UtcNow - epochStart;

        var serverTotalSeconds = Convert.ToUInt64(currentTs.TotalSeconds);
        var requestTotalSeconds = Convert.ToUInt64(requestTimestamp);

        var difference = serverTotalSeconds > requestTotalSeconds
            ? serverTotalSeconds - requestTotalSeconds
            : requestTotalSeconds - serverTotalSeconds;

        if (difference > RequestMaxAgeInSeconds)
        {
            _logger.LogWarning("Replay request detected - timeout. " +
                               "Server time {serverTotalSeconds}. " +
                               "Request time {requestTotalSeconds}. " +
                               "Difference {difference}",
                serverTotalSeconds, requestTotalSeconds, difference);
            return true;
        }

        await _cacheService.Set(nonce, requestTimestamp,
            _dateTimeProvider
                .UtcNowOffset
                .AddSeconds(RequestMaxAgeInSeconds));

        return false;
    }

    private static async Task<byte[]> ComputeHash(HttpRequest request)
    {
        var bodyBytes = await request.GetRawBodyBytesAsync();
        return bodyBytes.Length > 0
            ? MD5.Create().ComputeHash(bodyBytes)
            : null;
    }

    private (string appId,
        string incomingBase64Signature,
        string nonce,
        string requestTimestamp,
        bool skipBodyEncoding)?
        GetAuthorizationHeaderValues(string authorizationHeaderParameter)
    {
        var credentialsArray = authorizationHeaderParameter.Split(':');
        if (credentialsArray.Length is >= 4 and <= 5)
        {
            return (credentialsArray[0],
                credentialsArray[1],
                credentialsArray[2],
                credentialsArray[3],
                credentialsArray.Length is 5
                    && bool.TryParse(credentialsArray[4], out var isSkip) 
                    && isSkip);
        }

        _logger.LogWarning("Credentials array had unexpected length {credentialsArray.Length}",
            credentialsArray.Length);

        return null;
    }
}