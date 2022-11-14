using System.Security.Claims;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;

namespace Sfa.Tl.Find.Provider.Infrastructure.Extensions;

public static class ClaimsExtensions
{
    public static string? GetClaim(this ClaimsPrincipal user, string claim)
    {
        return user
                .FindFirst(c => c.Type.Equals(claim))
                ?.Value;
    }

    public static string GetUserSessionCacheKey(this ClaimsPrincipal user)
    {
        var claim = user.GetClaim(CustomClaimTypes.UserId);
        return CacheKeys.UserCacheKey(
            claim,
            CacheKeys.UserSessionActivityKey);
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
