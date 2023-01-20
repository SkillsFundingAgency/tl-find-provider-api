using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Web.Pages.Provider;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages.Provider;
public  class NotificationsTests
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
    public async Task NotificationsModel_OnGet_Populates_Notification_List_For_Administrator()
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
}
