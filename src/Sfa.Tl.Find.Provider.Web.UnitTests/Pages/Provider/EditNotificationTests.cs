using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages.Provider;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages.Provider;
public class EditNotificationTests
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

        var availableLocations = new NotificationLocationNameBuilder()
            .BuildList()
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotification(notificationId)
            .Returns(notification);
        providerDataService
            .GetAvailableNotificationLocationPostcodes(notificationId)
            .Returns(availableLocations);

        var editNotificationModel = new EditNotificationModelBuilder()
            .Build(providerDataService);

        await editNotificationModel.OnGet(notificationId);

        editNotificationModel.Notification.Should().Be(notification);
        editNotificationModel.HasAvailableLocations.Should().BeTrue();
        editNotificationModel.RemovedLocation.Should().BeNull();
    }

    [Fact]
    public async Task EditNotificationModel_OnGet_Populates_Notification_List()
    {
        const int notificationId = 1;

        var notification = new NotificationBuilder().Build();
        var notificationLocationSummaryList = new NotificationLocationSummaryBuilder()
            .BuildList()
            .ToList();

        var availableLocations = new NotificationLocationNameBuilder()
            .BuildList()
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotification(notificationId)
            .Returns(notification);
        providerDataService
            .GetNotificationLocationSummaryList(notificationId)
            .Returns(notificationLocationSummaryList);
        providerDataService
            .GetAvailableNotificationLocationPostcodes(notificationId)
            .Returns(availableLocations);

        var editNotificationModel = new EditNotificationModelBuilder()
            .Build(providerDataService);

        await editNotificationModel.OnGet(notificationId);

        editNotificationModel
            .NotificationLocationList
            .Should()
            .BeEquivalentTo(notificationLocationSummaryList);
    }

    [Fact]
    public async Task EditNotificationModel_OnGet_Sets_HasAvailableLocations_To_False_When_No_Available_Locations()
    {
        var notification = new NotificationBuilder()
            .WithSearchRadius(null)
            .Build();
        var notificationId = notification.Id!.Value;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotification(notificationId)
            .Returns(notification);
        providerDataService
            .GetAvailableNotificationLocationPostcodes(notificationId)
            .Returns(Enumerable.Empty<NotificationLocationName>());

        var editNotificationModel = new EditNotificationModelBuilder()
            .Build(providerDataService);

        await editNotificationModel.OnGet(notificationId);

        editNotificationModel.HasAvailableLocations.Should().BeFalse();
    }

    [Fact]
    public async Task EditNotificationModel_OnGet_Redirects_To_Notifications_If_Notification_Location_Not_Found()
    {
        const int notificationId = 999;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotification(notificationId)
            .Returns(null as Notification);

        var editNotificationModel = new EditNotificationModelBuilder()
            .Build(providerDataService);

        var result = await editNotificationModel.OnGet(notificationId);

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Provider/Notifications");
    }

    [Fact]
    public async Task EditNotificationModel_OnGetRemoveLocation_Deletes_From_Repository_And_Redirects()
    {
        const int notificationLocationId = 999;
        const int providerNotificationId = 1;

        var notificationLocation = new NotificationBuilder()
                .Build();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotificationLocation(notificationLocationId)
            .Returns(notificationLocation);

        var editNotificationModel = new EditNotificationModelBuilder()
            .Build(providerDataService);

        var result = await editNotificationModel.OnGetRemoveLocation(notificationLocationId, providerNotificationId);

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Provider/EditNotification");
        redirectResult.RouteValues.Should().Contain(x => 
            x.Key == "id" &&
            x.Value != null &&
            x.Value.ToString() == "1");

        await providerDataService
                .Received(1)
                .DeleteNotificationLocation(notificationLocationId);
    }

    [Fact]
    public async Task EditNotificationModel_OnGetRemoveLocation_Sets_TempData()
    {
        const int id = 999;
        const int providerNotificationId = 1;
        const string expectedLocation = "TEST LOCATION 1 [CV1 2WT]";

        var notification = new NotificationBuilder()
            .Build();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotificationLocation(id)
            .Returns(notification);

        var editNotificationModel = new EditNotificationModelBuilder()
            .Build(providerDataService);

        await editNotificationModel.OnGetRemoveLocation(id, providerNotificationId);

        editNotificationModel.TempData.Should().NotBeNull();
        editNotificationModel.TempData
            .Keys
            .Should()
            .Contain("RemovedLocation");

        editNotificationModel.TempData
            .Peek("RemovedLocation")
            .Should()
            .Be(expectedLocation);
    }

    [Fact]
    public async Task EditNotificationModel_OnGetRemoveLocation_Sets_TempData_For_Null_Location()
    {
        const int id = 999;
        const int providerNotificationId = 1;

        const string expectedLocation = "All";

        var notification = new NotificationBuilder()
            .WithNullLocation()
            .Build();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotificationLocation(id)
            .Returns(notification);

        var editNotificationModel = new EditNotificationModelBuilder()
            .Build(providerDataService);

        await editNotificationModel.OnGetRemoveLocation(id, providerNotificationId);

        editNotificationModel.TempData.Should().NotBeNull();
        editNotificationModel.TempData
            .Keys
            .Should()
            .Contain("RemovedLocation");

        editNotificationModel.TempData
            .Peek("RemovedLocation")
            .Should()
            .Be(expectedLocation);
    }
}
