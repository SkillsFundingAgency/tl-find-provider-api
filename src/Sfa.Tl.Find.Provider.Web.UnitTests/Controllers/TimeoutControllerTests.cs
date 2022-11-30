using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Application.Models.Session;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
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
    public async Task TimeoutController_ActivityTimeout_Clears_Cache()
    {
        var cacheService = Substitute.For<ICacheService>();

        var controller = new TimeoutControllerBuilder()
            .Build(cacheService);

        await controller.ActivityTimeout();

        cacheService
            .Received(1)
            .Remove(Arg.Is<string>(k => k.StartsWith("USERID")));
    }

    [Fact]
    public async Task TimeoutController_GetActiveDurationAsync_Returns_RedirectToPageResult()
    {
        var timeNowUtc = DateTime.SpecifyKind(DateTime.Parse("2022-11-01 10:21:42Z"), DateTimeKind.Utc);
        var previousTimeUtc = DateTime.SpecifyKind(DateTime.Parse("2022-11-01 10:11:12Z"), DateTimeKind.Utc);

        var cacheService = Substitute.For<ICacheService>();
        cacheService.Get<DateTime?>(Arg.Is<string>(k => k.StartsWith("USER")))
            .Returns(previousTimeUtc);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider
            .UtcNow
            .Returns(timeNowUtc);

        var signInSettings = new SettingsBuilder().BuildDfeSignInSettings(
            timeout: 20);

        var controller = new TimeoutControllerBuilder()
            .Build(cacheService, dateTimeProvider, signInSettings);

        var result = await controller.GetActiveDuration();

        var jsonResult = result as JsonResult;
        jsonResult.Should().NotBeNull();
        var data = jsonResult!.Value as SessionActivityData;
        data.Should().NotBeNull();
        data!.Minutes.Should().Be(9);
        data.Seconds.Should().Be(30);
    }

    [Fact]
    public async Task TimeoutController_RenewSessionActivity_Returns_RedirectToPageResult()
    {
        var timeNowUtc = DateTime.SpecifyKind(DateTime.Parse("2022-11-01 10:21:42Z"), DateTimeKind.Utc);

        var cacheService = Substitute.For<ICacheService>();

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider
            .UtcNow
            .Returns(timeNowUtc);

        var signInSettings = new SettingsBuilder().BuildDfeSignInSettings(
            timeout: 20);

        var controller = new TimeoutControllerBuilder()
            .Build(cacheService, dateTimeProvider, signInSettings);

        var result = await controller.RenewSessionActivity();

        result.Should().NotBeNull();

        var jsonResult = result as JsonResult;
        jsonResult.Should().NotBeNull();
        var data = jsonResult!.Value as SessionActivityData;
        data.Should().NotBeNull();
        data!.Minutes.Should().Be(20);
        data.Seconds.Should().Be(0);
    }

    [Fact]
    public async Task TimeoutController_RenewSessionActivity_Sets_Session_Cache()
    {
        var timeNowUtc = DateTime.SpecifyKind(DateTime.Parse("2022-11-01 10:21:42Z"), DateTimeKind.Utc);

        var cacheService = Substitute.For<ICacheService>();

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider
            .UtcNow
            .Returns(timeNowUtc);

        var signInSettings = new SettingsBuilder().BuildDfeSignInSettings(
            timeout: 20);

        var controller = new TimeoutControllerBuilder()
            .Build(cacheService, dateTimeProvider, signInSettings);

        await controller.RenewSessionActivity();

        cacheService
            .Received(1)
            .Set(Arg.Is<string>(k => k.StartsWith("USER")),
                timeNowUtc);
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
