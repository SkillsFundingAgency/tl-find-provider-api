using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using System.Security.Claims;
using Sfa.Tl.Find.Provider.Web.Pages.Provider;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class AddNotificationModelBuilder
{
    public AddNotificationModel Build(
        INotificationService? notificationService = null,
        IProviderDataService? providerDataService = null,
        ProviderSettings? providerSettings = null,
        ILogger<AddNotificationModel>? logger = null,
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
        notificationService ??= Substitute.For<INotificationService>();
        logger ??= Substitute.For<ILogger<AddNotificationModel>>();

        var providerOptions = Options.Create(
            providerSettings
            ?? new SettingsBuilder()
                .BuildProviderSettings());

        var tempDataProvider = Substitute.For<ITempDataProvider>();
        var tempData = new TempDataDictionary(
            pageContext.HttpContext,
            tempDataProvider);
        
        var pageModel = new AddNotificationModel(
            notificationService,
            providerDataService,
            providerOptions,
            logger)
        {
            PageContext = pageContext,
            TempData = tempData
        };

        return pageModel;
    }
}
