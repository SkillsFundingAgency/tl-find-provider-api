using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Services;
public class DfeSignInTokenServiceTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(DfeSignInTokenService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(DfeSignInTokenService)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public void GetDfeApiToken_Returns_Expected_Value()
    {
        var tokenService = new DfeSignInTokenServiceBuilder()
            .Build();

        var token = tokenService.GetApiToken();

        token.Should().NotBeNullOrEmpty();
        token.Length.Should().BeGreaterThan(1);
    }
}
