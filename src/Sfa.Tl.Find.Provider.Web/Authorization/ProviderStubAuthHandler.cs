﻿using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Sfa.Tl.Find.Provider.Web.Authorization;

public class ProviderStubAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProviderStubAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IHttpContextAccessor httpContextAccessor) : base(options, logger, encoder, clock)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, "10000001"),
            new Claim(CustomClaimTypes.DisplayName, "Test User"),
            new Claim(CustomClaimTypes.UkPrn, "10000001")
        };
        var identity = new ClaimsIdentity(claims, "Provider-stub");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Provider-stub");

        var result = AuthenticateResult.Success(ticket);

        _httpContextAccessor.HttpContext?.Items.Add(ClaimsIdentity.DefaultNameClaimType, "10000001");
        _httpContextAccessor.HttpContext?.Items.Add(CustomClaimTypes.DisplayName, "Test User");

        return Task.FromResult(result);
    }
}