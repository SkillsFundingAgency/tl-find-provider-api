using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Web.Pages;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class ErrorModelBuilder
{
    public ErrorModel Build(
            ILogger<ErrorModel>? logger = null,
            PageContext? pageContext = null)
    {
        pageContext ??= new PageContextBuilder()
            .Build();

        logger ??= Substitute.For<ILogger<ErrorModel>>();

        var pageModel = new ErrorModel(
            logger)
        {
            PageContext = pageContext
        };

        return pageModel;
    }
}
