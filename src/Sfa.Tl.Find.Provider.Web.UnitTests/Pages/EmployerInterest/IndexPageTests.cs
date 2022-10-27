using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages.EmployerInterest;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages.EmployerInterest;
public class IndexPageTests
{
    private const string TestPostcode = "CV1 2WT";

    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(IndexModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task IndexModel_OnGet_Sets_UkPrn()
    {
        var indexModel = new EmployerInterestIndexModelBuilder()
            .Build();

        await indexModel.OnGet();

        indexModel.UkPrn
            .Should()
            .Be(long.Parse(PageContextBuilder.DefaultUkPrn));
    }

    [Fact]
    public async Task IndexModel_OnGet_Sets_ExpectedProperties()
    {
        const int retentionDays = 84;
        const int expectedRetentionWeeks = 12;

        var settings = new SettingsBuilder().BuildEmployerInterestSettings(
            retentionDays: retentionDays);

        var indexModel = new EmployerInterestIndexModelBuilder()
            .Build(employerInterestSettings: settings);

        await indexModel.OnGet();

        indexModel.SearchRadius.Should().Be(settings.SearchRadius);
        indexModel.EmployerInterestRetentionDays.Should().Be(retentionDays);
        indexModel.EmployerInterestRetentionWeeks.Should().Be(expectedRetentionWeeks);

        var expectedServiceStartDate = DateOnly.FromDateTime(settings.ServiceStartDate!.Value);
        indexModel.ServiceStartDate.Should().Be(expectedServiceStartDate);
    }

    [Fact]
    public async Task IndexModel_OnGet_Sets_Expected_EmployerInterest_List()
    {
        var employerInterestSummary = new EmployerInterestSummaryBuilder()
            .BuildList()
            .ToList();

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .GetSummaryList()
            .Returns(employerInterestSummary);

        var indexModel = new EmployerInterestIndexModelBuilder()
            .Build(employerInterestService);

        await indexModel.OnGet();

        indexModel.EmployerInterestList
            .Should()
            .BeEquivalentTo(employerInterestSummary);
    }

    [Fact]
    public async Task IndexModel_OnGet_Sets_Expected_Provider_Locations_List()
    {
        var locationPostcodes = new LocationPostcodeBuilder()
            .BuildList()
            .ToList();

        var ukPrn = long.Parse(PageContextBuilder.DefaultUkPrn);
        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetLocationPostcodes(ukPrn)
            .Returns(locationPostcodes);

        var indexModel = new EmployerInterestIndexModelBuilder()
            .Build(providerDataService: providerDataService);

        await indexModel.OnGet();

        indexModel.ProviderLocations
            .Should()
            .BeEquivalentTo(locationPostcodes);
    }

    [Fact]
    public async Task IndexModel_OnPost_Sets_Expected_EmployerInterest_List()
    {
        var employerInterestSummary = new EmployerInterestSummaryBuilder()
            .BuildList()
            .ToList();

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .FindEmployerInterest(TestPostcode)
            .Returns((employerInterestSummary, employerInterestSummary.Count));

        var indexModel = new EmployerInterestIndexModelBuilder()
            .Build(employerInterestService);

        indexModel.Postcode = TestPostcode;

        await indexModel.OnPost();

        indexModel.EmployerInterestList
            .Should()
            .BeEquivalentTo(employerInterestSummary);
        indexModel.TotalEmployerInterestItems.Should().Be(employerInterestSummary.Count);
    }
}
