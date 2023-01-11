using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;

namespace Sfa.Tl.Find.Provider.Infrastructure.Extensions;

public static class ClaimsExtensions
{
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

    public static string? GetClaim(this ClaimsPrincipal user, string claim)
    {
        return user
                .FindFirst(c => c.Type.Equals(claim))
                ?.Value;
    }
    
    public static long? GetUkPrn(this ClaimsPrincipal user)
    {
        var ukPrnClaim = user.GetClaim(CustomClaimTypes.UkPrn);
        return ukPrnClaim is not null && long.TryParse(ukPrnClaim, out var ukPrn)
            ? ukPrn
            : null;
    }

    public static string GetUserSessionCacheKey(this ClaimsPrincipal user)
    {
        var claim = user.GetClaim(CustomClaimTypes.UserId);
        return CacheKeys.UserCacheKey(
            claim,
            CacheKeys.UserSessionActivityKey);
    }
}
