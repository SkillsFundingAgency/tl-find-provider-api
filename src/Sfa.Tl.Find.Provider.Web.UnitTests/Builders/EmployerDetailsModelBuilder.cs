using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Web.Pages;
using Sfa.Tl.Find.Provider.Web.Pages.Employer;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class EmployerDetailsModelBuilder
{
    public EmployerDetailsModel Build(
        IEmployerInterestService? employerInterestService = null,
            ILogger<EmployerDetailsModel>? logger = null,
            PageContext? pageContext = null,
            bool userIsAuthenticated = true)
    {
        pageContext ??= new PageContextBuilder()
            .Build(userIsAuthenticated);

        employerInterestService ??= Substitute.For<IEmployerInterestService>();
        logger ??= Substitute.For<ILogger<EmployerDetailsModel>>();

        var pageModel = new EmployerDetailsModel(
            employerInterestService,
            logger)
        {
            PageContext = pageContext
        };

        return pageModel;
    }
}
