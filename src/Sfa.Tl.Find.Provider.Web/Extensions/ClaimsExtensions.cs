using System.Security.Claims;

namespace Sfa.Tl.Find.Provider.Web.Extensions;

public static class ClaimsExtensions
{
    public static string? GetClaim(this ClaimsPrincipal user, string claim)
    {
        return user
                .FindFirst(c => c.Type.Equals(claim))
                ?.Value;
    }

    public static IList<Claim> AddIfNotNullOrEmpty(this IList<Claim> claims, 
        string claimType, 
        string? claim)
    {
        if (!string.IsNullOrEmpty(claim))
        {
            claims.Add(new Claim(claimType, claim));
        }
        return claims;
    }
}
