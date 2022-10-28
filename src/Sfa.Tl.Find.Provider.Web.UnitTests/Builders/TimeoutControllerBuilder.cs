using Microsoft.AspNetCore.Http;
using Sfa.Tl.Find.Provider.Web.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Caching.Memory;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class TimeoutControllerBuilder
{
    private const string DefaultUkPrn = "12345678";
    private const string DefaultOrganisationName = "Test Organisation";

    public TimeoutController Build(
        IMemoryCache? cache = null,
        DfeSignInSettings? signInSettings = null,
        ILogger<TimeoutController>? logger = null,
        bool userIsAuthenticated = true)
    {
        cache ??= Substitute.For<IMemoryCache>();

        var signInOptions = Options.Create(
            signInSettings
            ?? new SettingsBuilder()
                .BuildDfeSignInSettings());

        logger ??= Substitute.For<ILogger<TimeoutController>>();

        var authenticationService = Substitute.For<IAuthenticationService>();
        authenticationService
            .SignInAsync(Arg.Any<HttpContext>(), Arg.Any<string>(), Arg.Any<ClaimsPrincipal>(),
                Arg.Any<AuthenticationProperties>()).Returns(Task.FromResult((object)null!));

        var authSchemaProvider = Substitute.For<IAuthenticationSchemeProvider>();
        authSchemaProvider!.GetDefaultAuthenticateSchemeAsync()
            .Returns((new AuthenticationScheme(
                OpenIdConnectDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.DisplayName,
                typeof(IAuthenticationHandler))));

        var systemClock = Substitute.For<ISystemClock>();

        var services = new ServiceCollection();
        services.AddSingleton(authenticationService);
        services.AddSingleton(systemClock);
        services.AddSingleton(authSchemaProvider);

        var claims = new List<Claim>
        {
            new(CustomClaimTypes.UkPrn, DefaultUkPrn),
            new(CustomClaimTypes.OrganisationName, DefaultOrganisationName)
        };

        var httpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        };

        if (userIsAuthenticated)
        {
            httpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(claims,
                    AuthenticationExtensions.AuthenticationTypeName));
        }

        var controller = new TimeoutController(
            cache,
            signInOptions,
            logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        return controller;
    }
}
