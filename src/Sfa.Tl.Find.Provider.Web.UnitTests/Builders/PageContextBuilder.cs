using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using Sfa.Tl.Find.Provider.Web.Authorization;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class PageContextBuilder
{
    public const string DefaultUkPrn = "10000001";
    public const string DefaultNameClaimType = "10000001";
    public const string DefaultDisplayName = "Test User";
    public const string DefaultOrganisationName = "Test Organisation";

    private PageContext? _pageContext;
    private CompiledPageActionDescriptor? _actionDescriptor;

    public PageContext Build(
        bool userIsAuthenticated = true,
        List<Claim>? userClaims = null)
    {
        var httpContext = new DefaultHttpContext();

        if (userIsAuthenticated)
        {
            var claims = userClaims != null && userClaims.Any() 
                ? userClaims 
                : new List<Claim>
                {
                    new(CustomClaimTypes.UkPrn, DefaultUkPrn),
                    new(ClaimsIdentity.DefaultNameClaimType, DefaultNameClaimType),
                    new(CustomClaimTypes.DisplayName, DefaultDisplayName),
                    new(CustomClaimTypes.OrganisationName, DefaultOrganisationName)
                };

            httpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(claims,
                    AuthenticationExtensions.AuthenticationTypeName));
        }

        var modelState = new ModelStateDictionary();
        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new PageActionDescriptor(),
            modelState);
        var modelMetadataProvider = new EmptyModelMetadataProvider();
        var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);

        _pageContext = new PageContext(actionContext)
        {
            ViewData = viewData
        };

        if (_actionDescriptor != null)
        {
            _pageContext.ActionDescriptor = _actionDescriptor;
        }

        return _pageContext;
    }

    public PageContextBuilder WithViewEnginePath(string path)
    {
        _actionDescriptor ??= new CompiledPageActionDescriptor();
        _actionDescriptor.ViewEnginePath = path;

        if (_pageContext is not null)
        {
            _pageContext.ActionDescriptor = _actionDescriptor;
        }
        return this;
    }
}
