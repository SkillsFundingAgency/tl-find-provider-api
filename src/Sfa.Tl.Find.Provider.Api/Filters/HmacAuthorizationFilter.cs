using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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

        private const ulong RequestMaxAgeInSeconds = 300;
        private const string AuthenticationScheme = "amx";

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

            if (AllowedApps.Count == 0)
            {
                AllowedApps.TryAdd(apiSettings.Value.AppId, apiSettings.Value.ApiKey);
            }
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {

            try
            {
                if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader)
                    && authHeader[0].StartsWith(AuthenticationScheme))
                {
                    var credentials = authHeader[0].Substring(AuthenticationScheme.Length).Trim();
                    var authorizationHeaderArray = GetAuthorizationHeaderValues(credentials);

                    if (authorizationHeaderArray != null)
                    {
                        var appId = authorizationHeaderArray[0];
                        var incomingBase64Signature = authorizationHeaderArray[1];
                        var nonce = authorizationHeaderArray[2];
                        var requestTimeStamp = authorizationHeaderArray[3];

                        var isValid = await IsValidRequest(context.HttpContext.Request, appId, incomingBase64Signature, nonce, requestTimeStamp);

                        if (isValid)
                        {
                            //var currentPrincipal = new GenericPrincipal(new GenericIdentity(appId), null);
                            //context.Principal = currentPrincipal;
                            return;
                        }

                        context.Result = new UnauthorizedObjectResult("Incorrect 'Authorization' header.");
                    }
                }

                context.Result = new UnauthorizedObjectResult("Missing or malformed 'Authorization' header.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(HmacAuthorizationFilter)}", ex);
                throw;
            }
        }

        private async Task<bool> IsValidRequest(HttpRequest request, string appId, string incomingBase64Signature, string nonce, string requestTimeStamp)
        {
            var requestContentBase64String = "";
            
            var uri1 = request.GetDisplayUrl();
            var uri2 = request.GetEncodedUrl();
            //var requestUri = HttpUtility.UrlEncode(request.GetDisplayUrl().ToLower());
            var requestUri = request.GetDisplayUrl().ToLower();

            var requestHttpMethod = request.Method;

            if (!AllowedApps.ContainsKey(appId))
            {
                return false;
            }

            var sharedKey = AllowedApps[appId];

            if (IsReplayRequest(nonce, requestTimeStamp))
            {
                return false;
            }

            //var hash = requestHttpMethod == HttpMethod.Get.ToString()
            //        ? ComputeHash(requestUri)
            //        : await ComputeHash(request.Body);
            var hash =  await ComputeHash(request.Body);

            if (hash != null)
            {
                requestContentBase64String = Convert.ToBase64String(hash);
            }

            var data = $"{appId}{requestHttpMethod}{requestUri}{requestTimeStamp}{nonce}{requestContentBase64String}";

            //https://stackoverflow.com/questions/12185122/calculating-hmacsha256-using-c-sharp-to-match-payment-provider-example
            //var secretKeyBytes = Convert.FromBase64String(sharedKey);
            var secretKeyBytes = Encoding.ASCII.GetBytes(sharedKey);
            var secretKeyBytesUtf8 = Encoding.UTF8.GetBytes(sharedKey);

            var signatureAscii = Encoding.ASCII.GetBytes(data);
            var signature = Encoding.UTF8.GetBytes(data);

            //ASCII
            using var hmac = new HMACSHA256(secretKeyBytes);
            var signatureBytes = hmac.ComputeHash(signatureAscii);
            var base64Signature = Convert.ToBase64String(signatureBytes);

            //TODO: Pick one of these
            //UTF8
            using var hmac2 = new HMACSHA256(secretKeyBytesUtf8);
            var signatureBytes2 = hmac2.ComputeHash(signature);
            var base64Signature2 = Convert.ToBase64String(signatureBytes2);

            using var hmac3 = new HMACSHA256(secretKeyBytes);
            var signatureBytes3 = hmac3.ComputeHash(signature);
            var base64Signature3 = Convert.ToBase64String(signatureBytes3);
            
            var result1 = (incomingBase64Signature.Equals(base64Signature, StringComparison.Ordinal));
            var result2 = (incomingBase64Signature.Equals(base64Signature2, StringComparison.Ordinal));
            var result3 = (incomingBase64Signature.Equals(base64Signature3, StringComparison.Ordinal));

            return (incomingBase64Signature.Equals(base64Signature, StringComparison.Ordinal));
        }

        private bool IsReplayRequest(string nonce, string requestTimeStamp)
        {
            if (_cache.TryGetValue(nonce, out _))
            {
                return true;
            }

            var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            var currentTs = DateTime.UtcNow - epochStart;

            var serverTotalSeconds = Convert.ToUInt64(currentTs.TotalSeconds);
            var requestTotalSeconds = Convert.ToUInt64(requestTimeStamp);

            if (serverTotalSeconds - requestTotalSeconds > RequestMaxAgeInSeconds)
            {
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

        private static byte[] ComputeHash(string url)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(url);
            return inputBytes.Length != 0
                ? md5.ComputeHash(inputBytes)
                : null;
        }

        /*
        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var req = context.Request;

            if (req.Headers.Authorization != null && authenticationScheme.Equals(req.Headers.Authorization.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                var rawAuthzHeader = req.Headers.Authorization.Parameter;

                var autherizationHeaderArray = GetAutherizationHeaderValues(rawAuthzHeader);

                if (autherizationHeaderArray != null)
                {
                    var APPId = autherizationHeaderArray[0];
                    var incomingBase64Signature = autherizationHeaderArray[1];
                    var nonce = autherizationHeaderArray[2];
                    var requestTimeStamp = autherizationHeaderArray[3];

                    var isValid = isValidRequest(req, APPId, incomingBase64Signature, nonce, requestTimeStamp);

                    if (isValid.Result)
                    {
                        var currentPrincipal = new GenericPrincipal(new GenericIdentity(APPId), null);
                        context.Principal = currentPrincipal;
                    }
                    else
                    {
                        context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                    }
                }
                else
                {
                    context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                }
            }
            else
            {
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
            }

            return Task.FromResult(0);
        }
        */

        /*
        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            context.Result = new ResultWithChallenge(context.Result);
            return Task.FromResult(0);
        }
        */

        private string[] GetAuthorizationHeaderValues(string rawAuthorizationHeader)
        {
            var credArray = rawAuthorizationHeader.Split(':');

            if (credArray.Length == 4)
            {
                return credArray;
            }
            else
            {
                return null;
            }
        }
    }
}
