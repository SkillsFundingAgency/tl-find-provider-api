using System.Security.Claims;

namespace Sfa.Tl.Find.Provider.Web.Extensions;

public static class ClaimsExtensions
{
    public static string? GetClaim(this ClaimsPrincipal user, string claim)
    {
        //if (user == null) return null;

        return user
                .FindFirst(c => c.Type.Equals(claim))
                ?.Value;
    }
}
