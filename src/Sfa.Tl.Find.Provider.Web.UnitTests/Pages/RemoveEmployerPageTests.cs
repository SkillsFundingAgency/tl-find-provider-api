using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages.Employer;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages;
public class RemoveEmployerPageTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(RemoveEmployerModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task RemoveEmployerModel_OnGet_Sets_Expected_Results()
    {
        var employerInterestDetail = new EmployerInterestDetailBuilder()
            .Build();

        var id = employerInterestDetail.Id;

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
        .GetEmployerInterestDetail(id)
            .Returns(employerInterestDetail);

        var detailsModel = new RemoveEmployerModelBuilder()
            .Build(employerInterestService);

        await detailsModel.OnGet(id);

        detailsModel.EmployerInterest
            .Should()
            .BeEquivalentTo(employerInterestDetail);
    }

    [Fact]
    public async Task RemoveEmployerModel_OnGet_Redirects_To_404_If_Employer_Not_Found()
    {
        const int id = 999;

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .GetEmployerInterestDetail(id)
            .Returns(null as EmployerInterestDetail);

        var detailsModel = new RemoveEmployerModelBuilder()
            .Build(employerInterestService);

        var result = await detailsModel.OnGet(id);

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Error/404");
    }

    [Fact]
    public async Task RemoveEmployerModel_OnPost_Deletes_From_Repository_And_Redirects()
    {
        const int id = 999;

        var employerInterestDetail = new EmployerInterestDetailBuilder()
            .Build();

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .GetEmployerInterestDetail(id)
            .Returns(employerInterestDetail);

        var detailsModel = new RemoveEmployerModelBuilder()
            .Build(employerInterestService);

        var result = await detailsModel.OnPost(id);

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Employer/EmployerList");
        
        await employerInterestService
            .Received(1)
            .DeleteEmployerInterest(id);
    }

    [Fact]
    public async Task RemoveEmployerModel_OnPost_Sets_TempData()
    {
        const int id = 999;

        var employerInterestDetail = new EmployerInterestDetailBuilder()
            .Build();

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .GetEmployerInterestDetail(id)
            .Returns(employerInterestDetail);

        var detailsModel = new RemoveEmployerModelBuilder()
            .Build(employerInterestService);

        await detailsModel.OnPost(id);

        detailsModel.TempData.Should().NotBeNull();
        detailsModel.TempData
            .Keys
            .Should()
            .Contain("DeletedOrganisationName");

        detailsModel.TempData
            .Peek("DeletedOrganisationName")
            .Should()
            .Be(employerInterestDetail.OrganisationName);
    }
}
