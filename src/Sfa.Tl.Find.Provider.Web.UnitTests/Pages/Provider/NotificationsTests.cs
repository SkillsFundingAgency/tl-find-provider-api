using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages.Provider;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages.Provider;

public class NotificationsTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(NotificationsModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task NotificationsModel_OnGet_Returns_Page_Result()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var notificationsModel = new NotificationsModelBuilder()
            .Build(providerSettings: settings);

        var result = await notificationsModel.OnGet();
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task NotificationsModel_OnGet_With_Token_Returns_Redirect_Result()
    {
        const string token = "611c0ffc-8144-4ca5-9428-b2a555729947";

        var notificationsModel = new NotificationsModelBuilder()
            .Build();

        var result = await notificationsModel.OnGet(token);
        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Provider/Notifications");
    }
    
    [Fact]
    public async Task NotificationsModel_OnGet_With_Token_Calls_Service()
    {
        const string token = "611c0ffc-8144-4ca5-9428-b2a555729947";

        var providerDataService = Substitute.For<IProviderDataService>();

        var notificationsModel = new NotificationsModelBuilder()
            .Build(providerDataService);

        await notificationsModel.OnGet(token);

        await providerDataService
            .Received(1)
            .VerifyNotificationEmail(
                token);
    }

    [Fact]
    public async Task NotificationsModel_OnGet_Sets_ExpectedProperties()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var notificationsModel = new NotificationsModelBuilder()
            .Build(providerSettings: settings);

        await notificationsModel.OnGet();
    }

    [Fact]
    public async Task NotificationsModel_OnGet_Populates_Notification_List()
    {
        var notificationSummaryList = new NotificationSummaryBuilder()
            .BuildList()
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotificationSummaryList(PageContextBuilder.DefaultUkPrn)
            .Returns(notificationSummaryList);

        var notificationsModel = new NotificationsModelBuilder()
            .Build(providerDataService);

        await notificationsModel.OnGet();

        notificationsModel
            .NotificationList
            .Should()
            .BeEquivalentTo(notificationSummaryList);
    }

    [Fact]
    public async Task NotificationsModel_OnGetResendEmailVerification_Calls_Service()
    {
        var notification = new NotificationBuilder()
            .Build();
        var notificationId = notification.Id!.Value;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotification(notificationId)
            .Returns(notification);

        var notificationsModel = new NotificationsModelBuilder()
            .Build(providerDataService);

        await notificationsModel.OnGetResendEmailVerification(notificationId);

        await providerDataService
            .Received(1)
            .SendProviderVerificationEmail(notificationId, notification.Email);
    }
}