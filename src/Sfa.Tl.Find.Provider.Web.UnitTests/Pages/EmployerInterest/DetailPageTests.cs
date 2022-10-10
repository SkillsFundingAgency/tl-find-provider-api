using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages.EmployerInterest;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages.EmployerInterest;
public class DetailPageTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(DetailModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task DetailModel_OnGet_Sets_Expected_Results()
    {
        var employerInterest = new EmployerInterestBuilder()
            .Build();

        var id = employerInterest.Id;

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .GetEmployerInterest(id)
            .Returns(employerInterest);

        var detailModel = new EmployerInterestDetailModelBuilder()
            .Build(employerInterestService);

        await detailModel.OnGet(id);

        detailModel.EmployerInterest
            .Should()
            .BeEquivalentTo(employerInterest);
    }
}
