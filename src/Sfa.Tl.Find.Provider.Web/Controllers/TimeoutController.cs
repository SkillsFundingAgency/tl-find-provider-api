using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Web.Extensions;
using Sfa.Tl.Find.Provider.Application.Models.Session;

namespace Sfa.Tl.Find.Provider.Web.Controllers;

[Authorize]
public class TimeoutController : Controller
{
    private readonly DfeSignInSettings _signInSettings;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TimeoutController> _logger;

    public TimeoutController(
        IMemoryCache cache,
        IOptions<DfeSignInSettings> signInOptions,
        ILogger<TimeoutController> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _signInSettings = signInOptions?.Value
                          ?? throw new ArgumentNullException(nameof(signInOptions));
    }

    [HttpGet]
    [Route("active-duration", Name = "ActiveDuration")]
    public async Task<IActionResult> GetActiveDurationAsync()
    {
        var registeredSessionTime = _cache.Get<DateTime?>(User.GetUserSessionCacheKey());
        var remainingActiveDuration = 
            registeredSessionTime != null && registeredSessionTime != DateTime.MinValue 
                ? (registeredSessionTime.Value.AddMinutes(_signInSettings.Timeout) - DateTime.UtcNow) : new TimeSpan(0, 0, 0);
        return Json(new SessionActivityData { Minutes = remainingActiveDuration.Minutes < 0 ? 0 : remainingActiveDuration.Minutes, Seconds = remainingActiveDuration.Seconds < 0 ? 0 : remainingActiveDuration.Seconds });
    }

    [HttpGet]
    [Route("renew-activity", Name = "RenewSessionActivity")]
    public async Task<IActionResult> RenewSessionActivityAsync()
    {
        _cache.Set(User.GetUserSessionCacheKey(), DateTime.UtcNow);
        return Json(new SessionActivityData { Minutes = _signInSettings.Timeout, Seconds = 0 });
    }

    [HttpGet]
    [Route("activity-timeout", Name = "ActivityTimeout")]
    public async Task ActivityTimeout()
    {
        var userId = User.GetClaim(CustomClaimTypes.UserId);
        //TempData.Set(Constants.UserSessionActivityId, userId);
        _cache.Remove(User.GetUserSessionCacheKey());
        //TODO: Any redirect here?
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("timeout", Name = "Timeout")]
    public IActionResult TimeoutConfirmation()
    {
        //TODO: Do we want a separate Timeout page?
        return RedirectToPage(AuthenticationExtensions.UnauthenticatedUserStartPage);
    }

    //[AllowAnonymous]
    //[HttpGet]
    //[ActionName("SignIn")]
    //[Route("signin")]
    //public async Task SignIn()
    //{
    //    if (bool.TryParse(_configuration[Constants.SkipProviderAuthenticationConfigKey], out var isStubProviderAuth) &&
    //        isStubProviderAuth)
    //    {
    //        _logger.LogInformation("DfE Sign-in was not used. Redirecting to the dashboard.");
    //        Response.Redirect(AuthenticationExtensions.AuthenticatedUserStartPage);
    //    }
    //    else
    //    {
    //        await HttpContext.ChallengeAsync(
    //            new AuthenticationProperties
    //            {
    //                RedirectUri = AuthenticationExtensions.AuthenticatedUserStartPage
    //            });
    //    }
    //}

    //[HttpGet]
    //public IActionResult PostSignIn()
    //{
    //    return RedirectToPage(
    //        User.Identity is {IsAuthenticated: true} 
    //            ? AuthenticationExtensions.AuthenticatedUserStartPage 
    //            : AuthenticationExtensions.UnauthenticatedUserStartPage);
    //}

    //[HttpGet]
    //[ActionName("SignOut")]
    //[Route("signout")]
    //public new async Task<IActionResult> SignOut()
    //{
    //    if (bool.TryParse(_configuration[Constants.SkipProviderAuthenticationConfigKey], out var isStubProviderAuth) && isStubProviderAuth)
    //    {
    //        _logger.LogInformation("DfE Sign-in was not used. Signing out of fake authentication.");

    //        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    //        await HttpContext.SignOutAsync(AuthenticationExtensions.AuthenticationCookieName);
    //        return RedirectToPage("/SignedOut");
    //    }

    //    //TODO: Remove logging - just here for initial testing purposes
    //    _logger.LogInformation("User signed out of DfE Sign-in.");

    //    return SignOut(
    //        new AuthenticationProperties { RedirectUri = AuthenticationExtensions.UnauthenticatedUserStartPage },
    //        OpenIdConnectDefaults.AuthenticationScheme,
    //        CookieAuthenticationDefaults.AuthenticationScheme);
    //}

    //[AllowAnonymous]
    //[HttpGet]
    //[Route("signout-complete", Name = "SignOutComplete")]
    //public IActionResult SignoutComplete()
    //{
    //    //TODO: Remove logging - just here for initial testing purposes
    //    _logger.LogInformation("Signout complete from DfE Sign-in.");
    //    return Redirect(AuthenticationExtensions.UnauthenticatedUserStartPage);
    //}
}
