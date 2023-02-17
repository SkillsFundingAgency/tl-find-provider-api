using Sfa.Tl.Find.Provider.Infrastructure.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Authorization;

public static class AuthorizationPolicyExtension
{
    public static void AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.HasProviderAccount,
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(CustomClaimTypes.UkPrn);
                    policy.Requirements.Add(new ProviderRequirement());
                });

            options.AddPolicy(PolicyNames.IsAdministrator,
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new AdministratorRequirement());
                });

            options.AddPolicy(PolicyNames.IsProviderOrAdministrator,
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new ProviderOrAdministratorRequirement());
                });
        });
    }
}