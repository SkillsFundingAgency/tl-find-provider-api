using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Web.Pages;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Web.Authorization;

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
    public void IndexModel_OnGet_Returns_PageResult_When_User_Is_Not_Authenticated()
    {
        var indexModel = new IndexModelBuilder()
            .Build(userIsAuthenticated: false);

        var result = indexModel.OnGet();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be(AuthenticationExtensions.UnauthenticatedUserStartPage);
        indexModel.HttpContext.User.Identity!.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void IndexModel_OnGet_Returns_RedirectResult_When_User_Is_Authenticated()
    {
        var indexModel = new IndexModelBuilder().Build();

        var result = indexModel.OnGet();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().BeEquivalentTo(AuthenticationExtensions.AuthenticatedUserStartPage);
        indexModel.HttpContext.User.Identity!.IsAuthenticated.Should().BeTrue();
    }
}
