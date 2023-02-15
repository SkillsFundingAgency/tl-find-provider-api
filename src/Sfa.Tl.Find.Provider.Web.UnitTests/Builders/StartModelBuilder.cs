using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Web.Pages;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class StartModelBuilder
{
    public StartModel Build(
        ProviderSettings? providerSettings = null,
        ILogger<StartModel>? logger = null,
        PageContext? pageContext = null,
        bool userIsAuthenticated = true)
    {
        pageContext ??= new PageContextBuilder()
            .Build(userIsAuthenticated);

        logger ??= Substitute.For<ILogger<StartModel>>();

        var providerOptions = Options.Create(
            providerSettings
            ?? new SettingsBuilder()
                .BuildProviderSettings());

        var pageModel = new StartModel(
            providerOptions,
            logger)
        {
            PageContext = pageContext
        };

        return pageModel;
    }
}
