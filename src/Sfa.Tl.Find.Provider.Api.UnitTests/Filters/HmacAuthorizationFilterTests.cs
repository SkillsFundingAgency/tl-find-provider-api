using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Sfa.Tl.Find.Provider.Api.Filters;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Filters
{
    public class HmacAuthorizationFilterTests
    {
        private const string TestUri = "https://test.com/api/test";
        //private const string TestUri = "https://weblogs.asp.net/ricardoperes/unit-testing-the-httpcontext-in-controllers";

        [Fact]
        public void Constructor_Guards_Against_NullParameters()
        {
            typeof(HmacAuthorizationFilter)
                .ShouldNotAcceptNullConstructorArguments();
        }

        [Fact]
        public async Task Hmac_Authorization_Filter_Should_Fail_When_No_Authorization_Header()
        {
            var filter = new HmacAuthorizationFilterBuilder().Build();
            var context = new AuthorizationFilterContextBuilder().Build(TestUri);
            
            await filter.OnAuthorizationAsync(context);

            context.Result.Should().NotBeNull();

            var unauthorizedResult = context.Result as UnauthorizedObjectResult;
            context.HttpContext.Request.Headers.Should().NotContainKey("Authorization", "Invalid precondition");
            unauthorizedResult.Should().NotBeNull();
            unauthorizedResult!.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            unauthorizedResult!.Value.Should().Be("Missing or malformed 'Authorization' header.");
        }
        
        [Fact]
        public async Task Hmac_Authorization_Filter_Should_Fail_Authorization_Header_Is_Badly_Formed()
        {
            var filter = new HmacAuthorizationFilterBuilder().Build();
            var context = new AuthorizationFilterContextBuilder()
                .Build(TestUri, BuildInvalidAuthorizationHeader());

            await filter.OnAuthorizationAsync(context);

            context.Result.Should().NotBeNull();

            context.HttpContext.Request.Headers.Should().ContainKey("Authorization", "Invalid precondition");
            var unauthorizedResult = context.Result as UnauthorizedObjectResult;
            unauthorizedResult.Should().NotBeNull();
            unauthorizedResult!.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            unauthorizedResult!.Value.Should().Be("Missing or malformed 'Authorization' header.");
        }
        
        [Fact]
        public async Task Hmac_Authorization_Filter_Should_Pass_When_Authorization_Header_Is_Valid()
        {
            var apiSettings = new SettingsBuilder()
                .BuildApiSettings();

            var filter = new HmacAuthorizationFilterBuilder().Build(apiSettings);

            var context = new AuthorizationFilterContextBuilder()
                .Build(
                    TestUri,
                    BuildValidAuthorizationHeader(
                        "GET",
                        TestUri,
                        apiSettings.AppId, 
                        apiSettings.ApiKey));

            await filter.OnAuthorizationAsync(context);

            context.HttpContext.Request.Headers.Should().ContainKey("Authorization", "Invalid precondition");
            context.Result.Should().BeNull();
        }

        private IHeaderDictionary BuildInvalidAuthorizationHeader()
        {
            return new HeaderDictionary
            {
                { "Authorization", new StringValues("amx invalid") }
            };
        }

        private IHeaderDictionary BuildValidAuthorizationHeader(
            string method,
            string requestUri,
            string appId, 
            string apiKey)
        {
            var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            var timeSpan = DateTime.UtcNow - epochStart;
            var requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            var nonce = Guid.NewGuid().ToString("N");

            string requestContentBase64String = null;
            //if (request.Content != null)
            //{
            //    byte[] content = await request.Content.ReadAsByteArrayAsync();
            //    MD5 md5 = MD5.Create();
            //    //Hashing the request body, any change in request body will result in different hash, we'll incure message integrity
            //    byte[] requestContentHash = md5.ComputeHash(content);
            //    requestContentBase64String = Convert.ToBase64String(requestContentHash);
            //}

            var signatureRawData = $"{appId}{method}{requestUri.ToLower()}{requestTimeStamp}{nonce}{requestContentBase64String}";

            var secretKeyBytes = Encoding.ASCII.GetBytes(apiKey);
            var signature = Encoding.ASCII.GetBytes(signatureRawData);

            using var hmac = new HMACSHA256(secretKeyBytes);
            var signatureBytes = hmac.ComputeHash(signature);
            var requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

            return new HeaderDictionary
            {
                { "Authorization", new StringValues($"amx {appId}:{requestSignatureBase64String}:{nonce}:{requestTimeStamp}") }
            };
        }
    }
}
