using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.DelegatingHandlers
{
    public class HmacAuthorizationHeaderDelegatingHandler : DelegatingHandler
    {
        private readonly ApiSettings _apiSettings;

        public HmacAuthorizationHeaderDelegatingHandler(
            IOptions<ApiSettings> settings)
        {
            _apiSettings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestUri = request.RequestUri!.AbsoluteUri.ToLower();

            var requestHttpMethod = request.Method.Method;

            var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            var timeSpan = DateTime.UtcNow - epochStart;
            var requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            var nonce = Guid.NewGuid().ToString("N");

            string requestContentBase64String = null;
            if (request.Content != null)
            {
                var content = await request.Content.ReadAsByteArrayAsync(cancellationToken);
                using var md5 = MD5.Create();
                var requestContentHash = md5.ComputeHash(content);
                requestContentBase64String = Convert.ToBase64String(requestContentHash);
            }

            var signatureRawData = $"{_apiSettings.AppId}{requestHttpMethod}{requestUri.ToLower()}{requestTimeStamp}{nonce}{requestContentBase64String}";

            var secretKeyBytes = Encoding.ASCII.GetBytes(_apiSettings.ApiKey);
            var signature = Encoding.ASCII.GetBytes(signatureRawData);

            using var hmac = new HMACSHA256(secretKeyBytes);
            var signatureBytes = hmac.ComputeHash(signature);
            var requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

            request.Headers.Authorization = new AuthenticationHeaderValue("amx", $"{_apiSettings.AppId}:{requestSignatureBase64String}:{nonce}:{requestTimeStamp}");

            var response = await base.SendAsync(request, cancellationToken);

            return response;
        }
    }
}
