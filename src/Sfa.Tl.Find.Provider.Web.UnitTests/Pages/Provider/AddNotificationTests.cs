using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages.Provider;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages.Provider;
public  class AddNotificationTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(AddNotificationModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task AddNotificationModel_OnGet_Sets_ExpectedProperties()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var addNotificationsModel = new AddNotificationModelBuilder()
            .Build(providerSettings: settings);

        await addNotificationsModel.OnGet();
    }

    [Fact]
    public async Task AddNotification_OnPost_Saves_To_Repository_And_Redirects()
    {
        var notification = new NotificationBuilder()
            .Build();
        var notificationId = notification.Id!.Value;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotification(notificationId)
            .Returns(notification);

        var addNotificationsModel = new AddNotificationModelBuilder()
            .Build(providerDataService);
        
        addNotificationsModel.Input = new AddNotificationModel.InputModel
        {
            Email = notification.Email
        };

        var result = await addNotificationsModel.OnPost();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Provider/Notifications");

        await providerDataService
            .Received(1)
            .SaveNotification(Arg.Any<Notification>());
    }

    [Fact]
    public async Task AddNotification_OnPost_Sets_TempData()
    {
        var notification = new NotificationBuilder()
            .Build();

        var notificationId = notification.Id!.Value;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotification(notificationId)
            .Returns(notification);

        var addNotificationsModel = new AddNotificationModelBuilder()
            .Build(providerDataService);

        addNotificationsModel.Input = new AddNotificationModel.InputModel
        {
            Email = notification.Email
        };

        await addNotificationsModel.OnPost();

        addNotificationsModel.TempData.Should().NotBeNull();
        addNotificationsModel.TempData
            .Keys
            .Should()
            .Contain("AddedNotificationEmail");

        addNotificationsModel.TempData
            .Peek("AddedNotificationEmail")
            .Should()
            .Be(notification.Email);
    }
}
