using Microsoft.AspNetCore.Authorization;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Authorization;
public class ProviderOrAdministratorAuthorizationHandler : AuthorizationHandler<ProviderOrAdministratorRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProviderOrAdministratorRequirement requirement)
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
        var ukPrn = context
            .User
            .FindFirst(c => c.Type.Equals(
            CustomClaimTypes.UkPrn))?.Value;

        var isProvider = ukPrn is not null &&
               (context.User.IsInRole(CustomRoles.ProviderEndUser) ||
                context.User.IsInRole(CustomRoles.ProviderApprover));

        var isAdministrator = context.User.IsInRole(CustomRoles.Administrator);

        return isProvider || isAdministrator;
    }
}
