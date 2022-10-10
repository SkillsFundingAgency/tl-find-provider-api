using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Web.Pages.EmployerInterest;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class EmployerInterestDetailModelBuilder
{
    public DetailModel Build(
        IEmployerInterestService? employerInterestService = null,
            ILogger<DetailModel>? logger = null,
            PageContext? pageContext = null,
            bool userIsAuthenticated = true)
    {
        pageContext ??= new PageContextBuilder()
            .Build(userIsAuthenticated);

        employerInterestService ??= Substitute.For<IEmployerInterestService>();
        logger ??= Substitute.For<ILogger<DetailModel>>();

        var pageModel = new DetailModel(
            employerInterestService,
            logger)
        {
            PageContext = pageContext
        };

        return pageModel;
    }
}
