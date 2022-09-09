using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Controllers;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Controllers;
public class AccountControllerTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(AccountController)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task AccountController_On_XXXXX()
    {
        var controller = new AccountControllerBuilder().Build();

        //TODO: fill out tests;
    }
}
