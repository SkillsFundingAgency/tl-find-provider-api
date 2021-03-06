using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Api.Filters;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Filters;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Models;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Filters;

public class HmacAuthorizationFilterTests
{
    private const string TestUri = "https://test.com/api/test";

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

        var filter = new HmacAuthorizationFilterBuilder().Build(apiSettings);

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
}