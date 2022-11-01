using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages.Error;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages;
public class Error404PageTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(Error404Model)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Error404Model_OnGet_Populates_Page_Properties()
    {
        var indexModel = new Error404ModelBuilder().Build();

        indexModel.OnGet();
    }
}
