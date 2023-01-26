using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;
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

        var editNotificationModel = new EditNotificationModelBuilder()
            .Build(providerDataService, 
                providerSettings: settings);

        await editNotificationModel.OnGet(notificationId);

        editNotificationModel.Notification.Should().Be(notification);
    }

    [Fact]
    public async Task EditNotificationsModel_OnGet_Populates_Notification_List()
    {
        const int notificationId = 1;

        var notificationLocationSummaryList = new NotificationLocationSummaryBuilder()
            .BuildList()
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotificationLocationSummaryList(notificationId)
            .Returns(notificationLocationSummaryList);

        var editNotificationsModel = new EditNotificationModelBuilder()
            .Build(providerDataService);

        await editNotificationsModel.OnGet(notificationId);

        editNotificationsModel
            .NotificationLocationList 
            .Should()
            .BeEquivalentTo(notificationLocationSummaryList);
    }

    [Fact]
    public async Task EditNotificationModel_OnGet_Sets_Search_Radius_Select_List()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .Build();
        var id = notification.Id!.Value;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotification(id)
            .Returns(notification);

        var editNotificationModel = new EditNotificationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await editNotificationModel.OnGet(id);

        editNotificationModel.SearchRadiusOptions.Should().NotBeNullOrEmpty();
        var options = editNotificationModel.SearchRadiusOptions;

        options!.Length.Should().Be(6);
        options[0].Should().Match<SelectListItem>(x =>
            x.Text == "5 miles" && x.Value == "5");
        options[1].Should().Match<SelectListItem>(x =>
            x.Text == "10 miles" && x.Value == "10");
        options[2].Should().Match<SelectListItem>(x =>
            x.Text == "20 miles" && x.Value == "20");
        options[3].Should().Match<SelectListItem>(x =>
            x.Text == "30 miles" && x.Value == "30");
        options[4].Should().Match<SelectListItem>(x =>
            x.Text == "40 miles" && x.Value == "40");
        options[5].Should().Match<SelectListItem>(x =>
            x.Text == "50 miles" && x.Value == "50");
    }

    [Fact]
    public async Task EditNotificationModel_OnGet_Sets_Skill_Area_Select_List()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var routes = new RouteBuilder()
            .BuildList()
            .ToList();

        var notification = new NotificationBuilder()
            .Build();
        var id = notification.Id!.Value;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotification(id)
            .Returns(notification);
        providerDataService
            .GetRoutes()
            .Returns(routes);

        var editNotificationDetailModel = new EditNotificationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await editNotificationDetailModel.OnGet(id);

        editNotificationDetailModel.Input.Should().NotBeNull();
        editNotificationDetailModel.Input!.SkillAreas.Should().NotBeNullOrEmpty();
        var skillAreas = editNotificationDetailModel.Input.SkillAreas;

        skillAreas!.Length.Should().Be(routes.Count);

        skillAreas[0].Should().Match<SelectListItem>(x =>
            x.Text == "Agriculture, environment and animal care" && x.Value == "1");

        var orderedRoutes = routes.OrderBy(r => r.Name).ToArray();
        for (var i = 1; i < skillAreas.Length; i++)
        {
            skillAreas[i].Should().Match<SelectListItem>(x =>
                x.Text == orderedRoutes[i].Name &&
                x.Value == orderedRoutes[i].Id.ToString());
        }
    }

    [Fact]
    public async Task EditNotificationModel_OnGet_Sets_Initial_Input_Values_With_Default_Search_Radius()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .WithSearchRadius(null)
            .Build();
        var id = notification.Id!.Value;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotification(id)
            .Returns(notification);

        var editNotificationDetailModel = new EditNotificationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await editNotificationDetailModel.OnGet(id);

        editNotificationDetailModel.Input.Should().NotBeNull();
        //editNotificationDetailModel.Input!.NotificationId.Should().Be(id);
        editNotificationDetailModel.Input!.SelectedSearchRadius.Should().Be(settings.DefaultSearchRadius.ToString());
        editNotificationDetailModel.Input!.SelectedFrequency.Should().Be(NotificationFrequency.Immediately);
    }

    [Fact]
    public async Task EditNotificationModel_OnGet_Sets_Input_Selected_Values()
    {
        const int searchRadius = 30;

        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .WithSearchRadius(searchRadius)
            .Build();
        var id = notification.Id!.Value;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotification(id)
            .Returns(notification);

        var editNotificationDetailModel = new EditNotificationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await editNotificationDetailModel.OnGet(id);

        editNotificationDetailModel.Input.Should().NotBeNull();
        //editNotificationDetailModel.Input!.NotificationId.Should().Be(id);
        editNotificationDetailModel.Input!.SelectedSearchRadius.Should().Be(settings.DefaultSearchRadius.ToString());
        editNotificationDetailModel.Input!.SelectedFrequency.Should().Be(NotificationFrequency.Immediately);
    }

    [Fact]
    public async Task EditNotificationModel_OnGet_Redirects_To_404_If_Employer_Not_Found()
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

        var editNotificationModel = new EditNotificationModelBuilder()
            .Build(providerDataService);

        editNotificationModel.Input = new EditNotificationModel.InputModel
        {
            Id = notification.Id!.Value,
            SelectedSearchRadius = "30",
            SelectedFrequency = NotificationFrequency.Daily,
            SkillAreas = new[]
            {
                new SelectListItem("Value 1", "1", true)
            }
        };
        
        var result = await editNotificationModel.OnPost();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Provider/Notifications");

        await providerDataService
            .Received(1)
            .SaveNotification(Arg.Any<Notification>(),
                Arg.Any<long>());
    }
}
