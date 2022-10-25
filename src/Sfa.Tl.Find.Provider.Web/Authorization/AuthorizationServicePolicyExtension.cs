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
                        policy.RequireClaim(CustomClaimTypes.OrganisationCategory);
                        policy.Requirements.Add(new EmployerInterestViewerRequirement());
                    });

            options.AddPolicy(PolicyNames.EmployerInterestViewer,
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new EmployerInterestViewerRequirement());
                });
        });
    }
}