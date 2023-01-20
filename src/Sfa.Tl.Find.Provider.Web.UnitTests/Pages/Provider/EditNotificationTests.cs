using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages.Provider;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages.Provider;
public  class EditNotificationTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(EditNotificationModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task EditNotificationModel_OnGet_Sets_ExpectedProperties()
    {
        var notification = new NotificationBuilder()
            .Build();

        var notificationId = notification.Id!.Value;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotification(notificationId)
            .Returns(notification);

        var settings = new SettingsBuilder().BuildProviderSettings();

        var editNotificationsModel = new EditNotificationModelBuilder()
            .Build(providerDataService, 
                providerSettings: settings);

        await editNotificationsModel.OnGet(notificationId);

        editNotificationsModel.Notification.Should().Be(notification);
    }

    [Fact]
    public async Task EditNotificationModel_OnGet_Redirects_To_404_If_Employer_Not_Found()
    {
        const int notificationId = 999;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotification(notificationId)
            .Returns(null as Notification);

        var editNotificationsModel = new EditNotificationModelBuilder()
            .Build(providerDataService);

        var result = await editNotificationsModel.OnGet(notificationId);

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Error/404");
    }

    [Fact]
    public async Task EditNotification_OnPost_Saves_To_Repository_And_Redirects()
    {
        var notification = new NotificationBuilder()
            .Build();
        var notificationId = notification.Id!.Value;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotification(notificationId)
            .Returns(notification);

        var editNotificationsModel = new EditNotificationModelBuilder()
            .Build(providerDataService);

        editNotificationsModel.Input = new EditNotificationModel.InputModel
        {
            Id = notification.Id!.Value,
            Email = notification.Email
        };
        
        var result = await editNotificationsModel.OnPost();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Provider/Notifications");

        await providerDataService
            .Received(1)
            .SaveNotification(Arg.Any<Notification>());
    }
}
