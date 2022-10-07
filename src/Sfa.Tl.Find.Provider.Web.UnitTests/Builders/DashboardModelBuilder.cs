using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Web.Pages;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class DashboardModelBuilder
{
    public DashboardModel Build(
            ILogger<DashboardModel>? logger = null,
            PageContext? pageContext = null)
    {
        pageContext ??= new PageContextBuilder()
            .Build();

        logger ??= Substitute.For<ILogger<DashboardModel>>();

        var pageModel = new DashboardModel(
            logger)
        {
            PageContext = pageContext
        };

        return pageModel;
    }
}
