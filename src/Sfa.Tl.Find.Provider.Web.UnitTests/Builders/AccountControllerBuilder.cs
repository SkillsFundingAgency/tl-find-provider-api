using Microsoft.AspNetCore.Http;
using Sfa.Tl.Find.Provider.Web.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class AccountControllerBuilder
{
    public AccountController Build(
        DfeSignInSettings? signInSettings = null,
        ILogger<AccountController>? logger = null)
    {
        signInSettings ??= new SettingsBuilder().BuildDfeSignInSettings();

        var signInOptions = signInSettings.ToOptions();

        logger ??= Substitute.For<ILogger<AccountController>>();
        
        var claims = new List<Claim>
        {
            //new(ProviderClaims.ProviderUkprn, DefaultUkPrn),
            //new(ClaimsIdentity.DefaultNameClaimType, DefaultNameClaimType),
            //new(ProviderClaims.DisplayName, DefaultDisplayName),
            //new(ProviderClaims.Service, DefaultService)
        };

        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };
        
        var controller = new AccountController(
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
