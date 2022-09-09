using Sfa.Tl.Find.Provider.Web.Pages;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages;
public class IndexPageTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(IndexModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task IndexModel_OnGet_Populates_Page_Properties()
    {
        var indexModel = new IndexModelBuilder().Build();

        await indexModel.OnGet();
    }
}
