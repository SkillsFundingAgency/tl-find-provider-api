using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Controllers;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Controllers;
public class AccountControllerTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(AccountController)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task AccountController_SignIn_Challenges_Authentication()
    {
        var controller = new AccountControllerBuilder()
            .Build(userIsAuthenticated: false);

        //controller.ControllerContext.HttpContext.User.Should().BeNull();
        await controller.SignIn();

        //controller.ControllerContext.HttpContext.User.Should().NotBeNull();
        var authService = controller.ControllerContext.HttpContext.RequestServices
            .GetRequiredService<IAuthenticationService>();
        authService.Should().NotBeNull();

        await authService
            .Received(1)
            .ChallengeAsync(controller.ControllerContext.HttpContext,
                null,
                Arg.Is<AuthenticationProperties>(
                    p => p.RedirectUri == AuthenticationExtensions.AuthenticatedUserStartPage));

        controller.ControllerContext.HttpContext
            .Response
            .StatusCode
            .Should()
            .Be((int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task AccountController_SignIn_Redirects_When_DfeSignIn_Skipped()
    {
        var configuration = Substitute.For<IConfiguration>();
        configuration[Constants.SkipProviderAuthenticationConfigKey].Returns("true");

        var controller = new AccountControllerBuilder()
            .Build(configuration: configuration);

        await controller.SignIn();

        var authService = controller.ControllerContext.HttpContext.RequestServices
            .GetRequiredService<IAuthenticationService>();
        authService.Should().NotBeNull();

        await authService
            .DidNotReceive()
            .ChallengeAsync(Arg.Any<HttpContext>(),
                Arg.Any<string>(),
                Arg.Any<AuthenticationProperties>());

        controller.ControllerContext.HttpContext
            .Response
            .StatusCode
            .Should()
            .Be((int)HttpStatusCode.Redirect);

        controller.ControllerContext.HttpContext
            .Response
            .Headers
            .Should()
            .ContainKey("Location");

        controller.ControllerContext.HttpContext
            .Response
            .Headers["Location"].ToString()
            .Should()
            .Be(AuthenticationExtensions.AuthenticatedUserStartPage);
    }

    [Fact]
    public void AccountController_PostSignIn_Returns_RedirectResult()
    {
        var controller = new AccountControllerBuilder().Build();

        var result = controller.PostSignIn();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be(AuthenticationExtensions.AuthenticatedUserStartPage);
    }

    [Fact]
    public async Task AccountController_SignOut_Returns_SignOutResult()
    {
        var controller = new AccountControllerBuilder().Build();

        var result = await controller.SignOut();

        var signOutResult = result as SignOutResult;
        signOutResult.Should().NotBeNull();
        signOutResult!.AuthenticationSchemes.Should().Contain(CookieAuthenticationDefaults.AuthenticationScheme);
        signOutResult.AuthenticationSchemes.Should().Contain(OpenIdConnectDefaults.AuthenticationScheme);
    }

    [Fact]
    public async Task AccountController_SignOut_Returns_PageResult_When_DfeSignIn_Skipped()
    {
        var configuration = Substitute.For<IConfiguration>();
        configuration[Constants.SkipProviderAuthenticationConfigKey].Returns("true");

        var controller = new AccountControllerBuilder()
            .Build(configuration: configuration);

        var result = await controller.SignOut();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/SignedOut");
    }

    [Fact]
    public void AccountController_SignOutComplete_Returns_RedirectToPageResult()
    {
        var configuration = Substitute.For<IConfiguration>();
        configuration[Constants.SkipProviderAuthenticationConfigKey].Returns("true");

        var controller = new AccountControllerBuilder()
            .Build(configuration: configuration);

        var result = controller.SignoutComplete();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be(AuthenticationExtensions.UnauthenticatedUserStartPage);
    }
}
