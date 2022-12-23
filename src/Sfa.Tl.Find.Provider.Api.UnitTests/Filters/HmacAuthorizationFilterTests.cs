using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Api.Filters;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Filters;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Filters;

public class HmacAuthorizationFilterTests
{
    private const string TestUri = "https://test.com/api/test";

    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
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

        var header = new HmacAuthorizationHeaderBuilder()
            .WithInvalidHeader()
            .Build();

        var context = new AuthorizationFilterContextBuilder()
            .Build(TestUri, header);

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

        var filter = new HmacAuthorizationFilterBuilder()
            .Build(apiSettings);

        var header = new HmacAuthorizationHeaderBuilder()
            .WithAppId(apiSettings.AppId)
            .WithApiKey(apiSettings.ApiKey)
            .WithMethod(HttpMethod.Get)
            .WithUri(TestUri)
            .WithBody(null)
            .Build();

        var context = new AuthorizationFilterContextBuilder()
            .Build(TestUri, header);

        await filter.OnAuthorizationAsync(context);

        context.HttpContext.Request.Headers.Should().ContainKey("Authorization", "Invalid precondition");
        context.Result.Should().BeNull();
    }
    
    [Fact]
    public async Task Hmac_Authorization_Filter_Should_Pass_When_Request_Is_Post_With_Body()
    {
        var apiSettings = new SettingsBuilder()
            .BuildApiSettings();

        var filter = new HmacAuthorizationFilterBuilder()
            .Build(apiSettings);

        const string payload = "{ \"test\": 1 }";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

        var header = new HmacAuthorizationHeaderBuilder()
            .WithAppId(apiSettings.AppId)
            .WithApiKey(apiSettings.ApiKey)
            .WithMethod(HttpMethod.Post)
            .WithUri(TestUri)
            .WithBody(stream)
            .Build();

        var context = new AuthorizationFilterContextBuilder()
            .Build(TestUri, header, HttpMethod.Post.ToString(), stream);

        await filter.OnAuthorizationAsync(context);

        context.HttpContext.Request.Headers.Should().ContainKey("Authorization", "Invalid precondition");
        context.Result.Should().BeNull();
    }

    [Fact]
    public async Task Hmac_Authorization_Filter_Should_Set_Expiry_Time_In_Cache()
    {
        var timeSpan = DateTime.UtcNow - new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
        var requestTimestamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

        const string nonce = "E2BDD944A880499D99DC24E3CA750CDF";

        var apiSettings = new SettingsBuilder()
            .BuildApiSettings();

        var cacheService = Substitute.For<ICacheService>();

        var utcNow = DateTime.UtcNow;
        var baseOffset = new DateTimeOffset(utcNow);
        var expectedOffset = baseOffset.AddSeconds(300);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.UtcNow.Returns(utcNow);
        dateTimeProvider.UtcNowOffset.Returns(baseOffset);

        var filter = new HmacAuthorizationFilterBuilder()
            .Build(apiSettings, cacheService, dateTimeProvider);

        var header = new HmacAuthorizationHeaderBuilder()
            .WithAppId(apiSettings.AppId)
            .WithApiKey(apiSettings.ApiKey)
            .WithMethod(HttpMethod.Get)
            .WithUri(TestUri)
            .WithRequestTimestamp(requestTimestamp)
            .WithNonce(nonce)
            .WithBody(null)
            .Build();

        var context = new AuthorizationFilterContextBuilder()
            .Build(TestUri, header);

        await filter.OnAuthorizationAsync(context);

        context.HttpContext.Request.Headers.Should().ContainKey("Authorization", "Invalid precondition");
        context.Result.Should().BeNull();

        await cacheService
            .Received(1)
            .Set(
                nonce,
                requestTimestamp,
                expectedOffset);
    }
}