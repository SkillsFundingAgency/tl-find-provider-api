using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Web.Pages;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class EmployerListModelBuilder
{
    public EmployerListModel Build(
        IEmployerInterestService? employerInterestService = null,
        IProviderDataService? providerDataService = null,
        ILogger<EmployerListModel>? logger = null,
        PageContext? pageContext = null,
        bool userIsAuthenticated = true)
    {
        pageContext ??= new PageContextBuilder()
            .Build(userIsAuthenticated);

        employerInterestService ??= Substitute.For<IEmployerInterestService>();
        providerDataService ??= Substitute.For<IProviderDataService>();
        logger ??= Substitute.For<ILogger<EmployerListModel>>();

        var pageModel = new EmployerListModel(
            employerInterestService,
            providerDataService,
            logger)
        {
            PageContext = pageContext
        };

        return pageModel;
    }
}
