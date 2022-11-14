using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Controllers;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Controllers;
public class TimeoutControllerTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(TimeoutController)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void TimeoutController_ActivityTimeout_Returns_RedirectToPageResult()
    {
        var controller = new TimeoutControllerBuilder()
            .Build();

        var result = controller.ActivityTimeout();

        //Signs out
        //Clears cache
    }
    
    [Fact]
    public void TimeoutController_GetActiveDurationAsync_Returns_RedirectToPageResult()
    {
        var controller = new TimeoutControllerBuilder()
            .Build();

        var result = controller.GetActiveDuration();

        result.Should().NotBeNull();
        //var redirectResult = result as JsonResult;
        //redirectResult.Should().NotBeNull();
        //Returns 
    }

    [Fact]
    public void TimeoutController_RenewSessionActivity_Returns_RedirectToPageResult()
    {
        var controller = new TimeoutControllerBuilder()
            .Build();

        var result = controller.RenewSessionActivity();

        result.Should().NotBeNull();
    }
    [Fact]
    public void TimeoutController_e_Returns_RedirectToPageResult()
    {
        var controller = new TimeoutControllerBuilder()
            .Build();
    }
    [Fact]
    public void TimeoutController_SignOutComplete_Returns_RedirectToPageResult()
    {
        var controller = new TimeoutControllerBuilder()
            .Build();

        var result = controller.TimeoutConfirmation();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be(AuthenticationExtensions.UnauthenticatedUserStartPage);
    }

    //TimeoutConfirmation
    [Fact]
    public void TimeoutController_TimeoutConfirmation_Returns_RedirectToPageResult()
    {
        var controller = new TimeoutControllerBuilder()
            .Build();

        var result = controller.TimeoutConfirmation();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be(AuthenticationExtensions.UnauthenticatedUserStartPage);
    }
}
