using Microsoft.AspNetCore.Authorization;
using Sfa.Tl.Find.Provider.Web.Authorization;
using System.Security.Claims;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Extensions;

public class ProviderAuthorizationHandlerTests
{
    private const string TestUkPrn = "12345678";

    private readonly List<IAuthorizationRequirement> _authorizationRequirements = 
        new()
        {
            new ProviderUkPrnRequirement()
        };
    
    [Fact]
    public async Task Handler_Succeeds_When_UkPrn_Claim_Is_Present()
    {
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new List<Claim>
                {
                    new(CustomClaimTypes.UkPrn, 
                        TestUkPrn)
                },
                AuthenticationExtensions.AuthenticationTypeName));

        var context = new AuthorizationHandlerContext(
            _authorizationRequirements,
            user,
            null);

        var handler = new ProviderAuthorizationHandler();

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
        context.HasFailed.Should().BeFalse();
    }
    
    [Fact]
    public async Task HandleAsync_Fails_When_No_Claims_Present()
    {
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new List<Claim>(),
                AuthenticationExtensions.AuthenticationTypeName));

        var context = new AuthorizationHandlerContext(
            _authorizationRequirements,
            user,
            null);

        var handler = new ProviderAuthorizationHandler();

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
        context.HasFailed.Should().BeTrue();
    }
}