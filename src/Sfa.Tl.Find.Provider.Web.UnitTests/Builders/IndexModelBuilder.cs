using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Web.Pages;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class IndexModelBuilder
{
    public IndexModel Build(
            ILogger<IndexModel>? logger = null,
            PageContext? pageContext = null)
    {
        pageContext ??= new PageContextBuilder()
            .Build();

        logger ??= Substitute.For<ILogger<IndexModel>>();

        var pageModel = new IndexModel(
            logger)
        {
            PageContext = pageContext
        };

        return pageModel;
    }
}
