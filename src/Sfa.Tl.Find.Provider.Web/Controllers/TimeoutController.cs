using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Models.Session;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Services;

namespace Sfa.Tl.Find.Provider.Web.Controllers;

[Authorize]
public class TimeoutController : Controller
{
    private readonly DfeSignInSettings _signInSettings;
    private readonly ICacheService _cacheService;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILogger<TimeoutController> _logger;

    public TimeoutController(
        ICacheService cacheService,
        IDateTimeService dateTimeService,
        IOptions<DfeSignInSettings> signInOptions,
        ILogger<TimeoutController> logger)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _signInSettings = signInOptions?.Value
                          ?? throw new ArgumentNullException(nameof(signInOptions));
    }

    [HttpGet]
    [Route("active-duration", Name = "ActiveDuration")]
    public async Task<IActionResult> GetActiveDuration()
    {
        var registeredSessionTime = _cacheService.Get<DateTime?>(User.GetUserSessionCacheKey());
        var remainingActiveDuration = 
            registeredSessionTime != null && registeredSessionTime != DateTime.MinValue 
                ? (registeredSessionTime.Value.AddMinutes(_signInSettings.Timeout) - _dateTimeService.UtcNow) : new TimeSpan(0, 0, 0);
        return Json(new SessionActivityData { Minutes = remainingActiveDuration.Minutes < 0 ? 0 : remainingActiveDuration.Minutes, Seconds = remainingActiveDuration.Seconds < 0 ? 0 : remainingActiveDuration.Seconds });
    }

    [HttpGet]
    [Route("renew-activity", Name = "RenewSessionActivity")]
    public async Task<IActionResult> RenewSessionActivity()
    {
        _cacheService.Set(User.GetUserSessionCacheKey(), _dateTimeService.UtcNow);
        return Json(new SessionActivityData { Minutes = _signInSettings.Timeout, Seconds = 0 });
    }

    [HttpGet]
    [Route("activity-timeout", Name = "ActivityTimeout")]
    public async Task ActivityTimeout()
    {
        var userId = User.GetClaim(CustomClaimTypes.UserId);
        //TempData.Set(Constants.UserSessionActivityId, userId);
        _cacheService.Remove(User.GetUserSessionCacheKey());
        //TODO: Any redirect here?
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("timeout", Name = "Timeout")]
    public IActionResult TimeoutConfirmation()
    {
        return RedirectToPage(AuthenticationExtensions.UnauthenticatedUserStartPage);
    }
}
