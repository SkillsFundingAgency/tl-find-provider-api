using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using System.Security.Claims;
using Sfa.Tl.Find.Provider.Web.Pages.Provider;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class EditNotificationModelBuilder
{
    public EditNotificationModel Build(
        IProviderDataService? providerDataService = null,
        ISessionService? sessionService = null,
        ILogger<EditNotificationModel>? logger = null,
        PageContext? pageContext = null,
        bool userIsAuthenticated = true,
        bool isAdministrator = false)
    {
        var claims = userIsAuthenticated && isAdministrator
            ? new List<Claim>
            {
                new(ClaimTypes.Role, CustomRoles.Administrator)
            }
            : null;

        pageContext ??= new PageContextBuilder()
            .Build(userIsAuthenticated, claims);

        providerDataService ??= Substitute.For<IProviderDataService>();
        sessionService ??= Substitute.For<ISessionService>();
        logger ??= Substitute.For<ILogger<EditNotificationModel>>();

        var tempDataProvider = Substitute.For<ITempDataProvider>();
        var tempData = new TempDataDictionary(
            pageContext.HttpContext,
            tempDataProvider);
        
        var pageModel = new EditNotificationModel(
            providerDataService,
            sessionService,
            logger)
        {
            PageContext = pageContext,
            TempData = tempData
        };

        return pageModel;
    }
}
