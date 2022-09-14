using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Sfa.Tl.Find.Provider.Web.Controllers;

public class AccountController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly DfeSignInSettings _signInSettings;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IConfiguration configuration,
        IOptions<DfeSignInSettings> signInOptions,
        ILogger<AccountController> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        if (signInOptions is null) throw new ArgumentNullException(nameof(signInOptions));

        _signInSettings = signInOptions.Value;
    }

    [AllowAnonymous]
    [HttpGet]
    [ActionName("SignIn")]
    [Route("signin")]
    public async Task SignIn()
    {
        if (bool.TryParse(_configuration[Constants.SkipProviderAuthenticationConfigKey], out var isStubProviderAuth) &&
            isStubProviderAuth)
        {
            _logger.LogInformation("DfE Sign-in was not used. Redirecting to the dashboard.");
            Response.Redirect("/dashboard");
        }
        else
        {
            await HttpContext.ChallengeAsync(
                new AuthenticationProperties
                {
                    RedirectUri = "/dashboard"
                });
        }
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

    [HttpGet]
    [ActionName("SignOut")]
    [Route("signout")]
    public new async Task<IActionResult> SignOut()
    {
        if (bool.TryParse(_configuration[Constants.SkipProviderAuthenticationConfigKey], out var isStubProviderAuth) && isStubProviderAuth)
        {
            _logger.LogInformation("DfE Sign-in was not used. Signing out of fake authentication.");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(ProviderAuthenticationExtensions.AuthenticationCookieName);
            return RedirectToPage("/signedout");

            //return SignOut(
            //    new AuthenticationProperties
            //    {
            //        RedirectUri = "/signedout",
            //        //AllowRefresh = true
            //    },
            //    ProviderAuthenticationExtensions.AuthenticationCookieName,
            //    CookieAuthenticationDefaults.AuthenticationScheme);
        }

        //await HttpContext.SignOutAsync();
        //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return SignOut(
            OpenIdConnectDefaults.AuthenticationScheme,
            CookieAuthenticationDefaults.AuthenticationScheme);
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
