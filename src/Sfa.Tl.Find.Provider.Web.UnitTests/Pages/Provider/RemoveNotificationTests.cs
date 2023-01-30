using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages.Provider;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages.Provider;
public  class RemoveNotificationTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(RemoveNotificationModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task RemoveNotificationModel_OnGet_Sets_ExpectedProperties()
    {
        var notification = new NotificationBuilder()
            .Build();

        var notificationId = notification.Id!.Value;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotification(notificationId)
            .Returns(notification);

        var removeNotificationModel = new RemoveNotificationModelBuilder()
            .Build(providerDataService);

        await removeNotificationModel.OnGet(notificationId);

        removeNotificationModel.Notification
            .Should()
            .BeEquivalentTo(notification);
    }

    [Fact]
    public async Task RemoveNotificationModel_OnGet_Redirects_To_404_If_Employer_Not_Found()
    {
        const int id = 999;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotification(id)
            .Returns(null as Notification);

        var removeNotificationModel = new RemoveNotificationModelBuilder()
            .Build(providerDataService);

        var result = await removeNotificationModel.OnGet(id);

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Error/404");
    }

    [Fact]
    public async Task RemoveNotificationModel_OnPost_Deletes_From_Repository_And_Redirects()
    {
        const int id = 999;

        var notification = new NotificationBuilder()
            .Build();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotification(id)
            .Returns(notification);

        var removeNotificationModel = new RemoveNotificationModelBuilder()
            .Build(providerDataService);

        var result = await removeNotificationModel.OnPost(id);

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Provider/Notifications");

        await providerDataService
            .Received(1)
            .DeleteNotification(id);
    }

    [Fact]
    public async Task RemoveNotificationModel_OnPost_Sets_TempData()
    {
        const int id = 999;

        var notification = new NotificationBuilder()
            .Build();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotification(id)
            .Returns(notification);

        var removeNotificationModel = new RemoveNotificationModelBuilder()
            .Build(providerDataService);

        await removeNotificationModel.OnPost(id);

        removeNotificationModel.TempData.Should().NotBeNull();
        removeNotificationModel.TempData
            .Keys
            .Should()
            .Contain("DeletedNotificationEmail");

        removeNotificationModel.TempData
            .Peek("DeletedNotificationEmail")
            .Should()
            .Be(notification.Email);
    }
}
