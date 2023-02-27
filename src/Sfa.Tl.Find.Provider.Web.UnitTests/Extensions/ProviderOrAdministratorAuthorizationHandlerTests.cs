using Microsoft.AspNetCore.Authorization;
using Sfa.Tl.Find.Provider.Web.Authorization;
using System.Security.Claims;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Extensions;

public class ProviderOrAdministratorAuthorizationHandlerTests
{
    private const string TestUkPrn = "12345678";

    private readonly List<IAuthorizationRequirement> _authorizationRequirements =
        new()
        {
            new ProviderOrAdministratorRequirement()
        };

    [Fact]
    public async Task Handler_Succeeds_When_All_Claims_Present()
    {
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new List<Claim>
                {
                    new(CustomClaimTypes.UkPrn, TestUkPrn),
                    new(ClaimTypes.Role, CustomRoles.Administrator)
                },
                AuthenticationExtensions.AuthenticationTypeName));

        var context = new AuthorizationHandlerContext(
            _authorizationRequirements,
            user,
            null);

        var handler = new ProviderOrAdministratorAuthorizationHandler();

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
        context.HasFailed.Should().BeFalse();
    }

    [Fact]
    public async Task Handler_Succeeds_When_Administrator_Role_Claim_Is_Present()
    {
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new List<Claim>
                {
                    new(ClaimTypes.Role, CustomRoles.Administrator)
                },
                AuthenticationExtensions.AuthenticationTypeName));

        var context = new AuthorizationHandlerContext(
            _authorizationRequirements,
            user,
            null);

        var handler = new ProviderOrAdministratorAuthorizationHandler();

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
        context.HasFailed.Should().BeFalse();
    }
    
    [Fact]
    public async Task Handler_Fails_When_Administrator_Role_Claim_Is_Not_Present()
    {
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new List<Claim>
                {
                    new(ClaimTypes.Role, "Not A Role")
                },
                AuthenticationExtensions.AuthenticationTypeName));

        var context = new AuthorizationHandlerContext(
            _authorizationRequirements,
            user,
            null);

        var handler = new ProviderOrAdministratorAuthorizationHandler();

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
        context.HasFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handler_Succeeds_When_UkPrn_Claim_Is_Present()
    {
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new List<Claim>
                {
                    new(CustomClaimTypes.UkPrn, TestUkPrn),
                    new(ClaimTypes.Role, CustomRoles.ProviderEndUser)
                },
                AuthenticationExtensions.AuthenticationTypeName));

        var context = new AuthorizationHandlerContext(
            _authorizationRequirements,
            user,
            null);

        var handler = new ProviderOrAdministratorAuthorizationHandler();

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

        var handler = new ProviderOrAdministratorAuthorizationHandler();

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
        context.HasFailed.Should().BeTrue();
    }
}