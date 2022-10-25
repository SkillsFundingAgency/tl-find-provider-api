using Microsoft.AspNetCore.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Authorization;

public class ProviderAuthorizationHandler : AuthorizationHandler<ProviderUkPrnRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProviderUkPrnRequirement requirement)
    {
        if (!IsAuthorized(context))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        context.Succeed(requirement);

        return Task.CompletedTask;
    }

    private static bool IsAuthorized(AuthorizationHandlerContext context)
    {
        if (!context.User.HasClaim(c => c.Type.Equals(CustomClaimTypes.UkPrn)))
        {
            return false;
        }

        return true;
    }
}
