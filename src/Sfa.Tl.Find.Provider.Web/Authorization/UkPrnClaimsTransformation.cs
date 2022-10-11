using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Sfa.Tl.Find.Provider.Web.Authorization;

public class UkPrnClaimsTransformation : IClaimsTransformation
{
    public string? UkPrn { get; set; }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        //See https://gunnarpeipman.com/aspnet-core-adding-claims-to-existing-identity/
        //This requires the IClaimsTransformation to be registered as a singleton, so it remembers the UkPrn
        if (UkPrn is null)
        {
            return Task.FromResult(principal);
        }

        var clone = principal.Clone();
        var newIdentity = (ClaimsIdentity)clone.Identity;
        newIdentity.AddClaim(new Claim(CustomClaimTypes.UkPrn, UkPrn));
        
        return Task.FromResult(clone);
    }
}
