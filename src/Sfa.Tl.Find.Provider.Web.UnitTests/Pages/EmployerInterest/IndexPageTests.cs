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
    public async Task IndexModel_OnGet_Sets_Expected_Results()
    {
        var employerInterestSummary = new EmployerInterestSummaryItemBuilder()
            .BuildList()
            .ToList();

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .FindEmployerInterest()
            .Returns(employerInterestSummary);

        var indexModel = new EmployerInterestIndexModelBuilder()
            .Build(employerInterestService);

        await indexModel.OnGet();

        indexModel.EmployerInterestList
            .Should()
            .BeEquivalentTo(employerInterestSummary);
    }
}
