using Microsoft.AspNetCore.Http;
using Sfa.Tl.Find.Provider.Web.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class AccountControllerBuilder
{
    private const string DefaultUkPrn = "12345678";
    private const string DefaultOrganisationName = "Test Organisation";

    public AccountController Build(
        ICacheService? cacheService = null,
        ISessionService? sessionService = null,
        IConfiguration? configuration = null,
        ILogger<AccountController>? logger = null,
        bool userIsAuthenticated = true)
    {
        cacheService ??= Substitute.For<ICacheService>();
        sessionService ??= Substitute.For<ISessionService>();
        configuration ??= Substitute.For<IConfiguration>();

        logger ??= Substitute.For<ILogger<AccountController>>();

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

        var controller = new AccountController(
            cacheService,
            sessionService,
            configuration,
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
