using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Caching.Memory;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using ConfigurationConstants = Sfa.Tl.Find.Provider.Infrastructure.Configuration.Constants;

namespace Sfa.Tl.Find.Provider.Web.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;
    private readonly IMemoryCache _cache;

    public AccountController(
        IMemoryCache cache,
        IConfiguration configuration,
        ILogger<AccountController> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
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
            Response.Redirect(AuthenticationExtensions.AuthenticatedUserStartPage);
        }
        else
        {
            await HttpContext.ChallengeAsync(
                new AuthenticationProperties
                {
                    RedirectUri = AuthenticationExtensions.AuthenticatedUserStartPage
                });
        }
    }

    [HttpGet]
    public IActionResult PostSignIn()
    {
        //TODO: Move this into a filter
        //_cache.Set(User.GetUserSessionCacheKey(), DateTime.UtcNow);

        return RedirectToPage(
            User.Identity is {IsAuthenticated: true} 
                ? AuthenticationExtensions.AuthenticatedUserStartPage 
                : AuthenticationExtensions.UnauthenticatedUserStartPage);
    }

    [HttpGet]
    [ActionName("SignOut")]
    [Route("signout")]
    public new async Task<IActionResult> SignOut()
    {
        _cache.Remove(User.GetUserSessionCacheKey());

        if (bool.TryParse(_configuration[ConfigurationConstants.SkipProviderAuthenticationConfigKey], out var isStubProviderAuth) && isStubProviderAuth)
        {
            _logger.LogInformation("DfE Sign-in was not used. Signing out of fake authentication.");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(AuthenticationExtensions.AuthenticationCookieName);
            return RedirectToPage("/SignedOut");
        }

        //TODO: Remove logging - just here for initial testing purposes
        _logger.LogInformation("User signed out of DfE Sign-in.");

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
        //TODO: Remove logging - just here for initial testing purposes
        _logger.LogInformation("Signout complete from DfE Sign-in.");
        return Redirect(AuthenticationExtensions.UnauthenticatedUserStartPage);
    }
}
