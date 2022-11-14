using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Web.Pages;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class StartModelBuilder
{
    public StartModel Build(
        ILogger<StartModel>? logger = null,
        PageContext? pageContext = null,
        bool userIsAuthenticated = true)
    {
        pageContext ??= new PageContextBuilder()
            .Build(userIsAuthenticated);

        logger ??= Substitute.For<ILogger<StartModel>>();

        var pageModel = new StartModel(
            logger)
        {
            PageContext = pageContext
        };

        return pageModel;
    }
}
