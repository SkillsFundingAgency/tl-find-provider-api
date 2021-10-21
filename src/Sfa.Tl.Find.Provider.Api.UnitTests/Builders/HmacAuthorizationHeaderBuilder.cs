using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    public class HmacAuthorizationHeaderBuilder
    {
        private string _appId;
        private string _apiKey;
        private HttpMethod _method;
        private string _uri;
        private Stream _body;

        private readonly IHeaderDictionary _headers;

        private const string AuthorizationHeaderName = "Authorization";

        public HmacAuthorizationHeaderBuilder()
        {
            _headers = new HeaderDictionary();
        }

        public HmacAuthorizationHeaderBuilder RemoveHeader()
        {
            if (_headers.ContainsKey(AuthorizationHeaderName))
            {
                _headers.Remove(AuthorizationHeaderName);
            }
            return this;
        }
        
        public HmacAuthorizationHeaderBuilder WithInvalidHeader()
        {
            _headers[AuthorizationHeaderName] = new StringValues("amx invalid");
            return this;
        }

        public HmacAuthorizationHeaderBuilder WithAppId(string appId)
        {
            _appId = appId;
            return this;
        }

        public HmacAuthorizationHeaderBuilder WithApiKey(string apiKey)
        {
            _apiKey = apiKey;
            return this;
        }

        public HmacAuthorizationHeaderBuilder WithMethod(HttpMethod method)
        {
            _method = method;
            return this;
        }

        public HmacAuthorizationHeaderBuilder WithUri(string uri)
        {
            _uri = uri;
            return this;
        }

        public HmacAuthorizationHeaderBuilder WithBody(Stream body)
        {
            _body = body;
            return this;
        }

        public IHeaderDictionary Build()
        {
            if (_headers.ContainsKey(AuthorizationHeaderName))
            {
                return _headers;
            }

            var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            var timeSpan = DateTime.UtcNow - epochStart;
            var requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            var nonce = Guid.NewGuid().ToString("N");

            string requestContentBase64String = null;
            if (_body != null)
            {
                using var md5 = MD5.Create();
                var requestContentHash = md5.ComputeHash(_body);
                requestContentBase64String = Convert.ToBase64String(requestContentHash);
            }

            var signatureRawData = $"{_appId}{_method}{_uri?.ToLower()}{requestTimeStamp}{nonce}{requestContentBase64String}";

            var secretKeyBytes = Encoding.ASCII.GetBytes(_apiKey);
            var signature = Encoding.ASCII.GetBytes(signatureRawData);

            using var hmac = new HMACSHA256(secretKeyBytes);
            var signatureBytes = hmac.ComputeHash(signature);
            var requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

            _headers[AuthorizationHeaderName] =
                new StringValues($"amx {_appId}:{requestSignatureBase64String}:{nonce}:{requestTimeStamp}");
            
            return _headers;
        }
    }
}
