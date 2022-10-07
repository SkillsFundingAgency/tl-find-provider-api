using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Sfa.Tl.Find.Provider.Web.Authorization;

public static class ProviderStubAuthentication
{
    public static void AddProviderStubAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication("Provider-stub")
            .AddScheme<AuthenticationSchemeOptions, ProviderStubAuthHandler>(
                "Provider-stub",
                options => { })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(ProviderAuthenticationExtensions.AuthenticationCookieName);
    }
}