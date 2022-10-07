using Microsoft.AspNetCore.Authorization;

namespace Sfa.Tl.Find.Provider.Web.Authorization;

public class ProviderAuthorizationHandler : AuthorizationHandler<ProviderUkPrnRequirement>
{
    private const string UkPrnRootValue = "ukprn";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProviderAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProviderUkPrnRequirement requirement)
    {
        if (!IsProviderAuthorized(context))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        context.Succeed(requirement);

        return Task.CompletedTask;
    }

    private bool IsProviderAuthorized(AuthorizationHandlerContext context)
    {
        if (!context.User.HasClaim(c => c.Type.Equals(CustomClaimTypes.UkPrn)))
        {
            return false;
        }

        if (_httpContextAccessor.HttpContext != null 
            && _httpContextAccessor.HttpContext.Request.RouteValues.ContainsKey(UkPrnRootValue))
        {
            var ukPrnFromUrl = _httpContextAccessor.HttpContext.Request.RouteValues[UkPrnRootValue].ToString();
            var ukPrn = context.User.FindFirst(c => c.Type.Equals(CustomClaimTypes.UkPrn)).Value;

            return ukPrn.Equals(ukPrnFromUrl);
        }

        return true;
    }
}
