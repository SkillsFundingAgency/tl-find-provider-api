using Sfa.Tl.Find.Provider.Infrastructure.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Authorization;

public static class AuthorizationServicePolicyExtension
{
    public static void AddAuthorizationServicePolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.HasProviderAccount,
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(CustomClaimTypes.UkPrn);
                    policy.Requirements.Add(new ProviderUkPrnRequirement());
                });

            options.AddPolicy(PolicyNames.EmployerInterestViewer,
                    policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        //TODO: THis will require a role
                        policy.RequireClaim(CustomClaimTypes.OrganisationCategory);
                        policy.Requirements.Add(new EmployerInterestViewerRequirement());
                    });
        });
    }
}