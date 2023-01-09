using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using System.Security.Claims;
using Sfa.Tl.Find.Provider.Web.Pages.Provider;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class AddNotificationModelBuilder
{
    public AddNotificationModel Build(
        IProviderDataService? providerDataService = null,
        ISessionService? sessionService = null,
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
        sessionService ??= Substitute.For<ISessionService>();
        logger ??= Substitute.For<ILogger<AddNotificationModel>>();

        var providerOptions = Options.Create(
            providerSettings
            ?? new SettingsBuilder()
                .BuildProviderSettings());

        var pageModel = new AddNotificationModel(
            providerDataService,
            sessionService,
            providerOptions,
            logger)
        {
            PageContext = pageContext
        };

        return pageModel;
    }
}
