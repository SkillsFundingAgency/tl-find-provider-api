using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Web.Pages.Provider;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages.Provider;
public  class SearchFilterDetailsTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(SearchFilterDetailsModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task SearchFilterDetailsModel_OnGet_Sets_ExpectedProperties()
    {
        const int id = 999;
        const int expectedDefaultSearchRadius = 20;

        var settings = new SettingsBuilder().BuildProviderSettings();

        var searchFilterDetailsModel = new SearchFilterDetailsModelBuilder()
            .Build(providerSettings: settings);

        await searchFilterDetailsModel.OnGet(id);

        searchFilterDetailsModel.DefaultSearchRadius.Should().Be(expectedDefaultSearchRadius);
    }

    [Fact]
    public async Task SearchFilterDetailsModel_OnGet_Sets_Expected_Results()
    {
        var employerInterestDetail = new EmployerInterestDetailBuilder()
            .Build();

        var id = employerInterestDetail.Id;

        var searchFilter = new SearchFilterBuilder()
            .Build();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetSearchFilter(id)
            .Returns(searchFilter);

        var detailsModel = new SearchFilterDetailsModelBuilder()
            .Build(providerDataService);

        await detailsModel.OnGet(id);

        detailsModel.SearchFilter
            .Should()
            .BeEquivalentTo(searchFilter);
    }

    [Fact]
    public async Task SearchFilterDetailsModel_OnGet_Redirects_To_404_If_Employer_Not_Found()
    {
        const int id = 999;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetSearchFilter(id)
            .Returns(null as SearchFilter);

        var detailsModel = new SearchFilterDetailsModelBuilder()
            .Build(providerDataService);

        var result = await detailsModel.OnGet(id);

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Error/404");
    }
}
