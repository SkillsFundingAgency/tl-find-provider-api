using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Web.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Controllers;

public class AccountController : Controller
{
    private readonly DfeSignInSettings _signInSettings;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IOptions<DfeSignInSettings> signInOptions,
        ILogger<AccountController> logger)
    {
        if (signInOptions is null) throw new ArgumentNullException(nameof(signInOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _signInSettings = signInOptions.Value;
    }

    [AllowAnonymous]
    [HttpGet]//("/signin")]
    //[Route("signin", Name = "SignIn")]
    public async Task SignIn()
    {
        var returnUrl = "/dashboard";
        //var returnUrl = Url.Action(nameof(AccountController.PostSignIn), nameof(AccountController));
        await HttpContext.ChallengeAsync(new AuthenticationProperties { RedirectUri = returnUrl });
    }

    [HttpGet]
    public IActionResult PostSignIn()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToPage("/dashboard");
            //return !HttpContext.User.HasAccessToService()
            //    ? RedirectToAction("/errorController.ServiceAccessDenied), Constants.ErrorController)
            //    : RedirectToAction("/dashboard");
        }
        return RedirectToPage("/");
    }

    [HttpGet]//("/signout")]
    [Route("signout", Name = "SignOut")]
    //public async Task SignOut()
    //{
    //    await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
    //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    //}

    //[HttpGet("sign-out")]
    [ActionName("SignOut")]
    public IActionResult ProcessSignOut()
    {
        //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //return Redirect(_signInSettings.SignOutRedirectUri);

        return SignOut(
            CookieAuthenticationDefaults.AuthenticationScheme,
            ProviderAuthenticationExtensions.AuthenticationCookieName);
    }

    //[AllowAnonymous]
    //[HttpGet]
    //[Route("signout-complete", Name = RouteConstants.SignOutComplete)]
    //public IActionResult SignoutComplete()
    //{
    //    if (_signInSettings.SignOutRedirectUriEnabled && !string.IsNullOrWhiteSpace(_signInSettings.SignOutRedirectUri))
    //    {
    //        return Redirect(_signInSettings.SignOutRedirectUri);
    //    }
    //    else
    //    {
    //        return RedirectToAction(nameof(HomeController.Index), Constants.HomeController);
    //    }
    //}

    //[Authorize]
    //[HttpGet]
    //[Route("account-profile", Name = RouteConstants.AccountProfile)]
    //public IActionResult Profile()
    //{
    //    if (string.IsNullOrEmpty(_signInSettings.ProfileUrl))
    //    {
    //        _logger.LogWarning(LogEvent.ConfigurationMissing, $"Unable to read config: DfeSignInSettings.ProfileUrl, User: {User?.GetUserEmail()}");
    //        return RedirectToRoute("/Error/404");
    //    }
    //    return Redirect(_signInSettings.ProfileUrl);
    //}
}
