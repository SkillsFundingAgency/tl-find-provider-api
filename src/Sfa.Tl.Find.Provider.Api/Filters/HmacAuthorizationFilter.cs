using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Api.Filters
{
    public class HmacAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private static readonly ConcurrentDictionary<string, string> AllowedApps = new();

        private const string AuthenticationScheme = "amx";
        private const ulong RequestMaxAgeInSeconds = 300;

        private readonly IMemoryCache _cache;
        private readonly ILogger<HmacAuthorizationFilter> _logger;

        public HmacAuthorizationFilter(
            IOptions<ApiSettings> apiSettings,
            IMemoryCache cache,
            ILogger<HmacAuthorizationFilter> logger)
        {
            if (apiSettings is null) throw new ArgumentNullException(nameof(apiSettings));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (AllowedApps.IsEmpty)
            {
                AllowedApps.TryAdd(apiSettings.Value.AppId, apiSettings.Value.ApiKey);
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
                            credentials.Value.requestTimestamp);

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
                context.Result = new StatusCodeResult(500);
            }
        }
        
        private async Task<bool> IsValidRequest(HttpRequest request, string appId, string incomingBase64Signature, string nonce, string requestTimeStamp)
        {
            if (!AllowedApps.ContainsKey(appId))
            {
                _logger.LogWarning("Failed authentication - application id is not allowed.");
                return false;
            }

            if (IsReplayRequest(nonce, requestTimeStamp))
            {
                return false;
            }

            var requestContentBase64String = "";
            
            var hash = await ComputeHash(request.Body);
            if (hash != null)
            {
                requestContentBase64String = Convert.ToBase64String(hash);
            }

            var requestUri = request.GetDisplayUrl().ToLower();
            var sharedKey = AllowedApps[appId];

            var data = $"{appId}{request.Method.ToUpper()}{requestUri}{requestTimeStamp}{nonce}{requestContentBase64String}";

            var secretKeyBytes = Encoding.ASCII.GetBytes(sharedKey);
            var signature = Encoding.ASCII.GetBytes(data);
            using var hmac = new HMACSHA256(secretKeyBytes);
            var signatureBytes = hmac.ComputeHash(signature);
            var base64Signature = Convert.ToBase64String(signatureBytes);
            
            return incomingBase64Signature.Equals(base64Signature, StringComparison.Ordinal);
        }

        private bool IsReplayRequest(string nonce, string requestTimeStamp)
        {
            if (_cache.TryGetValue(nonce, out _))
            {
                _logger.LogWarning("Replay request detected - nonce found in cache.");
                return true;
            }

            var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            var currentTs = DateTime.UtcNow - epochStart;

            var serverTotalSeconds = Convert.ToUInt64(currentTs.TotalSeconds);
            var requestTotalSeconds = Convert.ToUInt64(requestTimeStamp);
            
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

            _cache.Set(nonce, requestTimeStamp,
                DateTimeOffset.UtcNow.AddSeconds(RequestMaxAgeInSeconds));

            return false;
        }

        private static async Task<byte[]> ComputeHash(Stream inputStream)
        {
            using var md5 = MD5.Create();
            return inputStream.CanRead && inputStream.CanSeek && inputStream.Length != 0 ?
                await md5.ComputeHashAsync(inputStream)
                : null;
        }

        private (string appId,
                 string incomingBase64Signature,
                 string nonce,
                 string requestTimestamp)?
                 GetAuthorizationHeaderValues(string authorizationHeaderParameter)
        {
            var credentialsArray = authorizationHeaderParameter.Split(':');
            if (credentialsArray.Length == 4)
            {
                return (credentialsArray[0],
                credentialsArray[1],
                credentialsArray[2],
                credentialsArray[3]);
            }

            _logger.LogWarning("Credentials array had unexpected length {credentialsArray.Length}", 
                credentialsArray.Length);

            return null;
        }
    }
}
