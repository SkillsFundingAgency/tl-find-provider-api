using Microsoft.AspNetCore.Authentication;

namespace Sfa.Tl.Find.Provider.Web.Authorization;

public static class ProviderStubAuthentication
{
    public static void AddProviderStubAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication("Provider-stub").AddScheme<AuthenticationSchemeOptions, ProviderStubAuthHandler>(
            "Provider-stub",
            options => { });
    }
}