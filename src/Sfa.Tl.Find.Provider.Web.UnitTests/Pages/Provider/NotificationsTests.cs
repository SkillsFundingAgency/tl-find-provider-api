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
        var notificationsModel = new NotificationsModelBuilder()
            .Build();

        var result = await notificationsModel.OnGet();
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task NotificationsModel_OnGet_With_Token_Returns_Redirect_Result()
    {
        const string token = "611c0ffc-8144-4ca5-9428-b2a555729947";
        const string email = "test@test.com";

        var notificationService = Substitute.For<INotificationService>();
        notificationService
            .VerifyNotificationEmail(token)
            .Returns((Success: true, Email: email));

        var notificationsModel = new NotificationsModelBuilder()
            .Build(notificationService);

        var result = await notificationsModel.OnGet(token);
        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Provider/Notifications");

        notificationsModel.TempData.Should().NotBeNull();
        notificationsModel.TempData
            .Keys
            .Should()
            .Contain("VerifiedEmail");

        notificationsModel.TempData
            .Peek("VerifiedEmail")
            .Should()
            .Be(email);
    }

    [Fact]
    public async Task NotificationsModel_OnGet_With_Token_Calls_Service()
    {
        const string token = "611c0ffc-8144-4ca5-9428-b2a555729947";

        var notificationService = Substitute.For<INotificationService>();

        var notificationsModel = new NotificationsModelBuilder()
            .Build(notificationService);

        await notificationsModel.OnGet(token);

        await notificationService
            .Received(1)
            .VerifyNotificationEmail(
                token);
    }

    [Fact]
    public async Task NotificationsModel_OnGet_Sets_ExpectedProperties()
    {
        var notificationsModel = new NotificationsModelBuilder()
            .Build();

        await notificationsModel.OnGet();

        notificationsModel.AddedNotificationEmail.Should().BeNull();
        notificationsModel.DeletedNotificationEmail.Should().BeNull();
        notificationsModel.VerificationEmail.Should().BeNull();
        notificationsModel.VerifiedEmail.Should().BeNull();
    }

    [Fact]
    public async Task NotificationsModel_OnGet_Populates_Notification_List()
    {
        var notificationSummaryList = new NotificationSummaryBuilder()
            .BuildList()
            .ToList();

        var notificationService = Substitute.For<INotificationService>();
        notificationService
            .GetNotificationSummaryList(PageContextBuilder.DefaultUkPrn)
            .Returns(notificationSummaryList);

        var notificationsModel = new NotificationsModelBuilder()
            .Build(notificationService);

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

        var notificationService = Substitute.For<INotificationService>();
        notificationService
            .GetNotification(notificationId)
            .Returns(notification);

        var notificationsModel = new NotificationsModelBuilder()
            .Build(notificationService);

        await notificationsModel.OnGetResendEmailVerification(notificationId);

        await notificationService
            .Received(1)
            .SendProviderNotificationVerificationEmail(notificationId, notification.Email);
    }
}
