using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Web.Pages.Error;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class Error404ModelBuilder
{
    public Error404Model Build(
            ILogger<Error404Model>? logger = null,
            PageContext? pageContext = null)
    {
        pageContext ??= new PageContextBuilder()
            .WithViewEnginePath("/Error/404")
            .Build();

        logger ??= Substitute.For<ILogger<Error404Model>>();

        var pageModel = new Error404Model(
            logger)
        {
            PageContext = pageContext
        };

        return pageModel;
    }
}
