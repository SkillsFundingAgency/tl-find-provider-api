using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Web.Pages;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

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
    public void IndexModel_OnGet_Populates_Page_Properties()
    {
        var pageModel = new IndexModel(Substitute.For<ILogger<IndexModel>>());

        pageModel.OnGet();

        //TODO: Add tests
    }
}
