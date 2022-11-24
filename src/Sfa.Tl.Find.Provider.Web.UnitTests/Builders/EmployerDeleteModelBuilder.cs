using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using Sfa.Tl.Find.Provider.Web.Pages.Employer;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class EmployerDeleteModelBuilder
{
    public EmployerDeleteModel Build(
        IEmployerInterestService? employerInterestService = null,
        ILogger<EmployerDetailsModel>? logger = null,
        PageContext? pageContext = null,
        bool userIsAuthenticated = true,
        bool isAdministrator = true)
    {
        var claims = userIsAuthenticated && isAdministrator
            ? new List<Claim>
            {
                new(ClaimTypes.Role, CustomRoles.Administrator)
            }
            : null;

        pageContext ??= new PageContextBuilder()
            .Build(userIsAuthenticated, claims);

        employerInterestService ??= Substitute.For<IEmployerInterestService>();
        logger ??= Substitute.For<ILogger<EmployerDetailsModel>>();

        var tempDataProvider = Substitute.For<ITempDataProvider>();
        var tempData = new TempDataDictionary(
            pageContext.HttpContext, 
            tempDataProvider);

        var pageModel = new EmployerDeleteModel(
            employerInterestService,
            logger)
        {
            PageContext = pageContext,
            TempData = tempData
        };

        return pageModel;
    }
}
