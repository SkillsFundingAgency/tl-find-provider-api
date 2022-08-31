using System.Security.Claims;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Authentication;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class PageContextBuilder
{
    public const string DefaultUkPrn = "10000001";
    public const string DefaultNameClaimType = "10000001";
    public const string DefaultDisplayName = "AED User";
    public const string DefaultService = "DAA";

    private PageContext? _pageContext;
    private CompiledPageActionDescriptor? _actionDescriptor;

    public PageContext Build()
    {
        var claims = new List<Claim>
        {
            new(ProviderClaims.ProviderUkprn, DefaultUkPrn),
            new(ClaimsIdentity.DefaultNameClaimType, DefaultNameClaimType),
            new(ProviderClaims.DisplayName, DefaultDisplayName),
            new(ProviderClaims.Service, DefaultService)
        };

        var identity = new ClaimsIdentity(claims);

        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };

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
