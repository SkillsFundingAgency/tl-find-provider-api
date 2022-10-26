using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages.EmployerInterest;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages.EmployerInterest;
public class IndexPageTests
{
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
}
