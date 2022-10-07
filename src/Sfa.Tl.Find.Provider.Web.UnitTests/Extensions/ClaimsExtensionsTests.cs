using Sfa.Tl.Find.Provider.Web.Extensions;
using System.Security.Claims;
using Sfa.Tl.Find.Provider.Web.Authorization;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Extensions;

public class ClaimsExtensionsTests
{
    private const string DefaultUkPrn = "12345678";

    [Fact]
    public void GetClaim_Returns_Null_For_Missing_Claim()
    {
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        
        var result = claimsPrincipal.GetClaim("missing");
        result.Should().BeNull();
    }

    [Fact]
    public void GetClaim_Returns_Expected_Value()
    {
        var claims = new List<Claim>
        {
            new(CustomClaimTypes.UkPrn, DefaultUkPrn)
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var result = claimsPrincipal.GetClaim(CustomClaimTypes.UkPrn);
        result.Should().Be(DefaultUkPrn);
    }
}