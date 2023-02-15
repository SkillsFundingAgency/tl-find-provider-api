using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages.Error;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages.Error;
public class Error403Tests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(Error403Model)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Error403Model_OnGet_Populates_Page_Properties()
    {
        var indexModel = new Error403ModelBuilder().Build();

        indexModel.OnGet();
    }
}
