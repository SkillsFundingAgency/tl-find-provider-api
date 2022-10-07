using Sfa.Tl.Find.Provider.Web.Pages;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages;
public class DashboardPageTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(DashboardModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task DashboardModel_OnGet_Populates_Page_Properties()
    {
        var dashboardModel = new DashboardModelBuilder().Build();

        await dashboardModel.OnGet();

        dashboardModel.DisplayName.Should().Be(PageContextBuilder.DefaultDisplayName);
        dashboardModel.Service.Should().Be(PageContextBuilder.DefaultService);
        dashboardModel.UkPrn.Should().Be(PageContextBuilder.DefaultUkPrn);
    }
}
