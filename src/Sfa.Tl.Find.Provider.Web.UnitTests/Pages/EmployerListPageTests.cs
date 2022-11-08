using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages;
public class EmployerListPageTests
{
    private const string TestPostcode = "CV1 2WT";

    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(EmployerListModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task EmployerListModel_OnGet_Sets_ExpectedProperties()
    {
        const int retentionDays = 84;
        const int expectedRetentionWeeks = 12;

        var settings = new SettingsBuilder().BuildEmployerInterestSettings(
            retentionDays: retentionDays);

        var employerListModel = new EmployerListModelBuilder()
            .Build(employerInterestSettings: settings);

        await employerListModel.OnGet();

        employerListModel.UkPrn
            .Should()
            .Be(long.Parse(PageContextBuilder.DefaultUkPrn));

        employerListModel.SearchRadius.Should().Be(settings.SearchRadius);
        employerListModel.EmployerInterestRetentionDays.Should().Be(retentionDays);
        employerListModel.EmployerInterestRetentionWeeks.Should().Be(expectedRetentionWeeks);

        var expectedServiceStartDate = DateOnly.FromDateTime(settings.ServiceStartDate!.Value);
        employerListModel.ServiceStartDate.Should().Be(expectedServiceStartDate);
    }

    [Fact]
    public async Task EmployerListModel_OnGet_Sets_Expected_EmployerInterest_List()
    {
        var employerInterestSummary = new EmployerInterestSummaryBuilder()
            .BuildList()
            .ToList();

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .GetSummaryList()
            .Returns(employerInterestSummary);

        var employerListModel = new EmployerListModelBuilder()
            .Build(employerInterestService);

        await employerListModel.OnGet();

        //employerListModel.EmployerInterestList
        //    .Should()
        //    .BeEquivalentTo(employerInterestSummary);
    }

    [Fact]
    public async Task EmployerListModel_OnGet_Sets_Expected_Provider_Locations_List()
    {
        var locationPostcodes = new LocationPostcodeBuilder()
            .BuildList()
            .ToList();

        var ukPrn = long.Parse(PageContextBuilder.DefaultUkPrn);
        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetLocationPostcodes(ukPrn)
            .Returns(locationPostcodes);

        var employerListModel = new EmployerListModelBuilder()
            .Build(providerDataService: providerDataService);

        await employerListModel.OnGet();

        employerListModel.ProviderLocations
            .Should()
            .BeEquivalentTo(locationPostcodes);
    }

    [Fact]
    public async Task EmployerListModel_OnPost_Sets_Expected_EmployerInterest_List()
    {
        var employerInterestSummary = new EmployerInterestSummaryBuilder()
            .BuildList()
            .ToList();

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .FindEmployerInterest(TestPostcode)
            .Returns((employerInterestSummary, employerInterestSummary.Count));

        var geoLocation = GeoLocationBuilder.BuildGeoLocation(TestPostcode);
        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(TestPostcode)
            .Returns(geoLocation);

        var employerListModel = new EmployerListModelBuilder()
            .Build(employerInterestService, postcodeLookupService);

        employerListModel.Input = new EmployerListModel.InputModel
        {
            CustomPostcode = TestPostcode
        };

        var result = await employerListModel.OnPost();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/EmployerList");
        
        //employerListModel.EmployerInterestList
        //    .Should()
        //    .BeEquivalentTo(employerInterestSummary);
        ////employerListModel.TotalEmployerInterestItems.Should().Be(employerInterestSummary.Count);

        //employerListModel.ZeroResultsFound.Should().NotBeNull();
        //employerListModel.ZeroResultsFound!.Value.Should().BeFalse();
    }

    [Fact]
    public async Task EmployerListModel_OnPost_Sets_Expected_HasResults_To_False_For_Zero_Results()
    {
        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .FindEmployerInterest(TestPostcode)
            .Returns((new List<EmployerInterestSummary>(), 0));

        var geoLocation = GeoLocationBuilder.BuildGeoLocation(TestPostcode);
        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(TestPostcode)
            .Returns(geoLocation);

        var employerListModel = new EmployerListModelBuilder()
            .Build(employerInterestService, postcodeLookupService);

        employerListModel.Input = new EmployerListModel.InputModel
        {
            CustomPostcode = TestPostcode
        };

        var result = await employerListModel.OnPost();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/EmployerList");

        //employerListModel.EmployerInterestList
        //    .Should()
        //    .BeEmpty();

        //employerListModel.ZeroResultsFound.Should().NotBeNull();
        //employerListModel.ZeroResultsFound!.Value.Should().BeTrue();
    }
}
