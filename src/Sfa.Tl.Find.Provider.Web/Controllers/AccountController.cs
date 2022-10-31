using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using ConfigurationConstants = Sfa.Tl.Find.Provider.Infrastructure.Configuration.Constants;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Web.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;
    private readonly ICacheService _cacheService;

    public AccountController(
        ICacheService cacheService,
        IConfiguration configuration,
        ILogger<AccountController> logger)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [AllowAnonymous]
    [HttpGet]
    [ActionName("SignIn")]
    [Route("signin")]
    public async Task SignIn()
    {
        if (bool.TryParse(_configuration[ConfigurationConstants.SkipProviderAuthenticationConfigKey], out var isStubProviderAuth) &&
            isStubProviderAuth)
        {
            _logger.LogInformation("DfE Sign-in was not used. Redirecting to the dashboard.");
            Response.Redirect(AuthenticationExtensions.AuthenticatedUserStartPageWithNoSlugs);
        }
        else
        {
            await HttpContext.ChallengeAsync(
                new AuthenticationProperties
                {
                    RedirectUri = AuthenticationExtensions.AuthenticatedUserStartPageWithSlugs
                });
        }
    }

    [HttpGet]
    public IActionResult PostSignIn()
    {
        return RedirectToPage(
            User.Identity is {IsAuthenticated: true} 
                ? AuthenticationExtensions.AuthenticatedUserStartPageWithNoSlugs 
                : AuthenticationExtensions.UnauthenticatedUserStartPage);
    }

    [HttpGet]
    [ActionName("SignOut")]
    [Route("signout")]
    public new async Task<IActionResult> SignOut()
    {
        _cacheService.Remove(User.GetUserSessionCacheKey());

        if (bool.TryParse(_configuration[ConfigurationConstants.SkipProviderAuthenticationConfigKey], out var isStubProviderAuth) && isStubProviderAuth)
        {
            _logger.LogInformation("DfE Sign-in was not used. Signing out of fake authentication.");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(AuthenticationExtensions.AuthenticationCookieName);
            return RedirectToPage("/SignedOut");
        }

        return SignOut(
            new AuthenticationProperties { RedirectUri = AuthenticationExtensions.UnauthenticatedUserStartPage },
            OpenIdConnectDefaults.AuthenticationScheme,
            CookieAuthenticationDefaults.AuthenticationScheme);
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("signout-complete", Name = "SignOutComplete")]
    public IActionResult SignoutComplete()
    {
        return Redirect(AuthenticationExtensions.UnauthenticatedUserStartPage);
    }
}
