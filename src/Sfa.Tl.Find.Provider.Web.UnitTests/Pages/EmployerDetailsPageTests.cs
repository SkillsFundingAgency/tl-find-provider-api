using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages.Employer;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages;
public class EmployerDetailsPageTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(EmployerDetailsModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task EmployerDetailsModel_OnGet_Sets_Expected_Results()
    {
        var employerInterestDetail = new EmployerInterestDetailBuilder()
            .Build();

        var id = employerInterestDetail.Id;

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
        .GetEmployerInterestDetail(id)
            .Returns(employerInterestDetail);

        var detailsModel = new EmployerDetailsModelBuilder()
            .Build(employerInterestService);

        await detailsModel.OnGet(id);

        detailsModel.EmployerInterest
            .Should()
            .BeEquivalentTo(employerInterestDetail);
    }

    [Fact]
    public async Task EmployerDetailsModel_OnGet_Redirects_To_404_If_Employer_Not_Found()
    {
        const int id = 999;

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .GetEmployerInterestDetail(id)
            .Returns(null as EmployerInterestDetail);

        var detailsModel = new EmployerDetailsModelBuilder()
            .Build(employerInterestService);

        var result = await detailsModel.OnGet(id);

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Error/404");
    }
}
