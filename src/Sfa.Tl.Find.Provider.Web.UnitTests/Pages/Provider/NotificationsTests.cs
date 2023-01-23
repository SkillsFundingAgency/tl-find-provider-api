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
            .NotBeNullOrEmpty();

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