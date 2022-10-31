using Microsoft.AspNetCore.Authorization;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Authorization;

public class EmployerInterestViewerAuthorizationHandler : AuthorizationHandler<EmployerInterestViewerRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployerInterestViewerRequirement requirement)
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
        var organisationCategory = context.User.Claims.FirstOrDefault(c => c.Type.Equals(CustomClaimTypes.OrganisationCategory));
        var ukPrn = context.User.FindFirst(c => c.Type.Equals(CustomClaimTypes.UkPrn))?.Value;

        if (ukPrn is not null)
        {
            if (string.IsNullOrEmpty(ukPrn))
            {
                //If this happens, we need to remove it
            }
            return true;
        }

        //If has either a ukprn or a matching category...
        if (organisationCategory is not null
            && int.TryParse(organisationCategory.Value, out var categoryId)
            && categoryId is 2 or 11) //Local Authority or Government
        {
            return true;
        }

        return false;
    }
}
