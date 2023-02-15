using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Models.Session;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Web.Controllers;

//[Authorize]
public class TimeoutController : Controller
{
    private readonly DfeSignInSettings _signInSettings;
    private readonly ICacheService _cacheService;
    private readonly IDateTimeProvider _dateTimeProvider;
    // ReSharper disable once NotAccessedField.Local
    private readonly ILogger<TimeoutController> _logger;

    public TimeoutController(
        ICacheService cacheService,
        IDateTimeProvider dateTimeProvider,
        IOptions<DfeSignInSettings> signInOptions,
        ILogger<TimeoutController> logger)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _signInSettings = signInOptions?.Value
                          ?? throw new ArgumentNullException(nameof(signInOptions));
    }

    [HttpGet]
    [Route("active-duration", Name = "ActiveDuration")]
    public async Task<IActionResult> GetActiveDuration()
    {
        var registeredSessionTime = await _cacheService.Get<DateTime?>(User.GetUserSessionCacheKey());
        var remainingActiveDuration =
            registeredSessionTime != null && registeredSessionTime != DateTime.MinValue
                ? registeredSessionTime.Value.AddMinutes(_signInSettings.Timeout) - _dateTimeProvider.UtcNow 
                : new TimeSpan(0, 0, 0);
        
        return Json(new SessionActivityData { Minutes = remainingActiveDuration.Minutes < 0 ? 0 : remainingActiveDuration.Minutes, Seconds = remainingActiveDuration.Seconds < 0 ? 0 : remainingActiveDuration.Seconds });
    }

    [HttpGet]
    [Route("renew-activity", Name = "RenewSessionActivity")]
    public async Task<IActionResult> RenewSessionActivity()
    {
        await _cacheService.Set(User.GetUserSessionCacheKey(), _dateTimeProvider.UtcNow);
        return Json(new SessionActivityData { Minutes = _signInSettings.Timeout, Seconds = 0 });
    }

    [HttpGet]
    [Route("activity-timeout", Name = "ActivityTimeout")]
    public async Task ActivityTimeout()
    {
        await _cacheService.Remove<DateTime>(User.GetUserSessionCacheKey());

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
