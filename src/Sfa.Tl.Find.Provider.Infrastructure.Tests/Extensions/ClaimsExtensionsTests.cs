using System.Security.Claims;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;

namespace Sfa.Tl.Find.Provider.Infrastructure.Tests.Extensions;

public class ClaimsExtensionsTests
{
    private const string TestUkPrn = "12345678";
    private const string TestUrn = "123456";

    [Fact]
    public void AddIfNotNullOrEmpty_Returns_Expected_Value()
    {
        var claims = new List<Claim>();

        claims
            .AddIfNotNullOrEmpty(CustomClaimTypes.UkPrn, TestUkPrn)
            .AddIfNotNullOrEmpty(CustomClaimTypes.Urn, TestUrn);

        claims.Count.Should().Be(2);
        claims.Should().Contain(c =>
            c.Type == CustomClaimTypes.UkPrn);
        claims.Should().Contain(c =>
            c.Type == CustomClaimTypes.UkPrn && c.Value == TestUkPrn);
        claims.Should().Contain(c =>
            c.Type == CustomClaimTypes.Urn && c.Value == TestUrn);
    }

    [Fact]
    public void AddIfNotNullOrEmpty_Should_Not_Add_Null_Value()
    {
        var claims = new List<Claim>();

        claims
            .AddIfNotNullOrEmpty(CustomClaimTypes.UkPrn, null)
            .AddIfNotNullOrEmpty(CustomClaimTypes.Urn, "");

        claims.Count.Should().Be(0);
    }

    [Fact]
    public void AddIfNotNullOrEmpty_Should_Not_Add_Empty_Value()
    {
        var claims = new List<Claim>();

        claims
            .AddIfNotNullOrEmpty(CustomClaimTypes.Urn, "");

        claims.Count.Should().Be(0);
    }

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
            new(CustomClaimTypes.UkPrn, TestUkPrn)
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var result = claimsPrincipal.GetClaim(CustomClaimTypes.UkPrn);
        result.Should().Be(TestUkPrn);
    }
}