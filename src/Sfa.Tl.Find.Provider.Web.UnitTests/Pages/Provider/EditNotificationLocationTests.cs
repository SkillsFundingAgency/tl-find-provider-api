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
public class EditNotificationLocationTests
{
    private const int ProviderNotificationId = 1;
    private const int NotificationLocationId = 10;

    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(EditNotificationLocationModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task EditNotificationLocationModel_OnGet_Sets_ExpectedProperties()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var editNotificationLocationModel = new EditNotificationLocationModelBuilder()
            .Build(providerSettings: settings);

        await editNotificationLocationModel.OnGet(ProviderNotificationId, NotificationLocationId);
    }

    [Fact]
    public async Task EditNotificationLocationModel_OnGet_Sets_FrequencyOptions_Select_List()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .Build();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotificationLocation(notification.Id!.Value)
            .Returns(notification);

        var editNotificationLocationModel = new EditNotificationLocationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await editNotificationLocationModel.OnGet(ProviderNotificationId, notification.Id.Value);

        editNotificationLocationModel.SearchRadiusOptions.Should().NotBeNullOrEmpty();
        var options = editNotificationLocationModel.FrequencyOptions;

        options!.Length.Should().Be(3);
        options[0].Should().Match<SelectListItem>(x =>
            x.Text == "Immediately" && x.Value == "1");
        options[1].Should().Match<SelectListItem>(x =>
            x.Text == "Daily" && x.Value == "2");
        options[2].Should().Match<SelectListItem>(x =>
            x.Text == "Weekly" && x.Value == "3");
    }

    [Fact]
    public async Task EditNotificationLocationModel_OnGet_Sets_Search_Radius_Select_List()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .Build();
        
        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotificationLocation(notification.Id!.Value)
            .Returns(notification);

        var editNotificationLocationModel = new EditNotificationLocationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await editNotificationLocationModel.OnGet(ProviderNotificationId, notification.Id.Value);

        editNotificationLocationModel.SearchRadiusOptions.Should().NotBeNullOrEmpty();
        var options = editNotificationLocationModel.SearchRadiusOptions;

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
    public async Task EditNotificationLocationModel_OnGet_Sets_Skill_Area_Select_List()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .Build();

        var routes = new RouteBuilder()
            .BuildList()
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotificationLocation(notification.Id!.Value)
            .Returns(notification);
        providerDataService
            .GetRoutes()
            .Returns(routes);

        var editNotificationLocationModel = new EditNotificationLocationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await editNotificationLocationModel.OnGet(ProviderNotificationId, notification.Id.Value);

        editNotificationLocationModel.Input.Should().NotBeNull();
        editNotificationLocationModel.Input!.SkillAreas.Should().NotBeNullOrEmpty();
        var skillAreas = editNotificationLocationModel.Input.SkillAreas;

        skillAreas!.Length.Should().Be(routes.Count);

        skillAreas[0].Should().Match<SelectListItem>(x =>
            x.Text == "Agriculture, environment and animal care" && x.Value == "1");
        
        var i = 0;
        foreach (var route in routes.OrderBy(r => r.Name))
        {
            skillAreas[i].Should().Match<SelectListItem>(x =>
                x.Text == route.Name &&
                x.Value == route.Id.ToString());
            i++;
        }
    }

    [Fact]
    public async Task EditNotificationLocationModel_OnGet_Sets_Selected_Skill_Areas()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .Build();

        var routes = new RouteBuilder()
            .BuildList()
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotificationLocation(notification.Id!.Value)
            .Returns(notification);
        providerDataService
            .GetRoutes()
            .Returns(routes);

        var editNotificationLocationModel = new EditNotificationLocationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await editNotificationLocationModel.OnGet(ProviderNotificationId, notification.Id.Value);

        var skillAreas = editNotificationLocationModel.Input?.SkillAreas;
        skillAreas.Should().NotBeNull();
        var selectedSkillAreas = skillAreas!.Where(x => x.Selected).ToList();
        selectedSkillAreas.Count.Should().Be(notification.Routes.Count);

        foreach (var s in notification.Routes)
        {
            selectedSkillAreas.Should().Contain(x => x.Value == s.Id.ToString());
        }
    }

    [Fact]
    public async Task EditNotificationLocationModel_OnGet_Sets_Initial_Input_Values_With_Default_Search_Radius()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .WithSearchRadius(null)
            .Build();
        
        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotificationLocation(notification.Id!.Value)
            .Returns(notification);

        var editNotificationLocationModel = new EditNotificationLocationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await editNotificationLocationModel.OnGet(ProviderNotificationId, notification.Id.Value);

        editNotificationLocationModel.Input.Should().NotBeNull();
        editNotificationLocationModel.Input!.Id.Should().Be(notification.Id!.Value);
        editNotificationLocationModel.Input!.ProviderNotificationId.Should().Be(ProviderNotificationId);
        editNotificationLocationModel.Input!.SelectedSearchRadius.Should().Be(settings.DefaultNotificationSearchRadius);
        editNotificationLocationModel.Input!.SelectedFrequency.Should().Be(notification.Frequency);
        //editNotificationLocationModel.Input!.SelectedLocation.Should().Be(0);
    }

    [Fact]
    public async Task EditNotificationLocationModel_OnGet_Sets_Input_Selected_Values()
    {
        const int searchRadius = 30;

        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .WithSearchRadius(searchRadius)
            .Build();
        
        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotificationLocation(notification.Id!.Value)
            .Returns(notification);

        var editNotificationLocationModel = new EditNotificationLocationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await editNotificationLocationModel.OnGet(ProviderNotificationId, notification.Id.Value);

        editNotificationLocationModel.Input.Should().NotBeNull();
        editNotificationLocationModel.Input!.Id.Should().Be(notification.Id!.Value);
        editNotificationLocationModel.Input!.ProviderNotificationId.Should().Be(ProviderNotificationId);
        editNotificationLocationModel.Input!.SelectedSearchRadius.Should().Be(notification.SearchRadius);
        editNotificationLocationModel.Input!.SelectedFrequency.Should().Be(notification.Frequency);
    }

    [Fact]
    public async Task EditNotificationLocationModel_OnGet_Redirects_To_EditNotification_If_Notification_Location_Not_Found()
    {
        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotificationLocation(NotificationLocationId)
            .Returns(null as Notification);

        var editNotificationLocationModel = new EditNotificationLocationModelBuilder()
            .Build(providerDataService);

        var result = await editNotificationLocationModel.OnGet(ProviderNotificationId, NotificationLocationId);

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Provider/EditNotification");
        redirectResult.RouteValues.Should().Contain(x =>
            x.Key == "id" &&
            x.Value != null &&
            x.Value.ToString() == ProviderNotificationId.ToString());
    }

    [Fact]
    public async Task EditNotificationLocationModel_OnPost_Saves_To_Repository_And_Redirects()
    {
        var notification = new NotificationBuilder()
            .Build();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetNotificationLocation(notification.Id!.Value)
            .Returns(notification);

        var editNotificationLocationModel = new EditNotificationLocationModelBuilder()
            .Build(providerDataService);

        editNotificationLocationModel.Input = new EditNotificationLocationModel.InputModel
        {
            Id = notification.Id!.Value,
            ProviderNotificationId = ProviderNotificationId,
            SelectedSearchRadius = 30,
            SelectedFrequency = NotificationFrequency.Daily,
            SkillAreas = new[]
            {
                new SelectListItem("Value 1", "1", true)
            }
        };

        var result = await editNotificationLocationModel.OnPost();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Provider/EditNotification");
        redirectResult.RouteValues.Should().Contain(x =>
            x.Key == "id" &&
            x.Value != null &&
            x.Value.ToString() == $"{ProviderNotificationId}");

        await providerDataService
            .Received(1)
            .SaveNotificationLocation(Arg.Any<Notification>());
    }
}
