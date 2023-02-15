using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages;
public class ErrorTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(ErrorModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void ErrorModel_OnGet_Populates_Page_Properties()
    {
        var indexModel = new ErrorModelBuilder().Build();

        indexModel.OnGet();
    }
}
