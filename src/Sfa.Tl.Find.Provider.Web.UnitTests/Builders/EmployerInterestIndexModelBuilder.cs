using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Web.Pages.EmployerInterest;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class EmployerInterestIndexModelBuilder
{
    public IndexModel Build(
        IEmployerInterestService? employerInterestService = null,
        IProviderDataService? providerDataService = null,
        ILogger<IndexModel>? logger = null,
        PageContext? pageContext = null,
        bool userIsAuthenticated = true)
    {
        pageContext ??= new PageContextBuilder()
            .Build(userIsAuthenticated);

        employerInterestService ??= Substitute.For<IEmployerInterestService>();
        providerDataService ??= Substitute.For<IProviderDataService>();
        logger ??= Substitute.For<ILogger<IndexModel>>();

        var pageModel = new IndexModel(
            employerInterestService,
            providerDataService,
            logger)
        {
            PageContext = pageContext
        };

        return pageModel;
    }
}
