﻿using Microsoft.AspNetCore.Authorization;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Authorization;

public class ProviderAuthorizationHandler : AuthorizationHandler<ProviderRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProviderRequirement requirement)
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

        return ukPrn is not null && 
               (context.User.IsInRole(CustomRoles.ProviderEndUser) ||
               context.User.IsInRole(CustomRoles.ProviderApprover));
    }
}
