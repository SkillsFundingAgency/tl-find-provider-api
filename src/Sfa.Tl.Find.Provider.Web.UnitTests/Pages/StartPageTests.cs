using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Web.Pages;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages;
public class StartPageTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(StartModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void StartModel_OnGet_Returns_PageResult_When_User_Is_Not_Authenticated()
    {
        var startModel = new StartModelBuilder()
            .Build(userIsAuthenticated: false);

        var result = startModel.OnGet();

        result.Should().NotBeNull();

        var pageResult = result as PageResult;
        pageResult.Should().NotBeNull();
        startModel.HttpContext.User.Identity!.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void StartModel_OnGet_Returns_RedirectResult_When_User_Is_Authenticated()
    {
        var startModel = new StartModelBuilder().Build();

        var result = startModel.OnGet();
        
        result.Should().NotBeNull();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        startModel.HttpContext.User.Identity!.IsAuthenticated.Should().BeTrue();
    }
}
