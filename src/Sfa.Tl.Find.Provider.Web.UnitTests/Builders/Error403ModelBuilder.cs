using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Web.Pages.Error;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class Error403ModelBuilder
{
    public Error403Model Build(
            ILogger<Error403Model>? logger = null,
            PageContext? pageContext = null)
    {
        pageContext ??= new PageContextBuilder()
            .WithViewEnginePath("/Error/403")
            .Build();

        logger ??= Substitute.For<ILogger<Error403Model>>();

        var pageModel = new Error403Model(
            logger)
        {
            PageContext = pageContext
        };

        return pageModel;
    }
}
