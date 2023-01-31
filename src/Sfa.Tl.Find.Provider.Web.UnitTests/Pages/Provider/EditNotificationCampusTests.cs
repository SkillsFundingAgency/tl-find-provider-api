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
public class EditNotificationCampusTests
{
    private const string EmailFieldKey = "Input.Email";

    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(EditNotificationCampusModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task EditNotificationCampusModel_OnGet_Sets_ExpectedProperties()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var editNotificationCampusModel = new EditNotificationCampusModelBuilder()
            .Build(providerSettings: settings);

        await editNotificationCampusModel.OnGet();

        editNotificationCampusModel.DefaultSearchRadius.Should().Be(settings.DefaultSearchRadius);
    }

    [Fact]
    public async Task EditNotificationCampusModel_OnGet_Sets_FrequencyOptions_Select_List()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .Build();
        //var id = notification.Id!.Value;

        var providerDataService = Substitute.For<IProviderDataService>();
        //providerDataService
        //    .GetNotification(id)
        //    .Returns(notification);

        var editNotificationCampusModel = new EditNotificationCampusModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await editNotificationCampusModel.OnGet();

        editNotificationCampusModel.SearchRadiusOptions.Should().NotBeNullOrEmpty();
        var options = editNotificationCampusModel.FrequencyOptions;

        options!.Length.Should().Be(3);
        options[0].Should().Match<SelectListItem>(x =>
            x.Text == "Immediately" && x.Value == "1");
        options[1].Should().Match<SelectListItem>(x =>
            x.Text == "Daily" && x.Value == "2");
        options[2].Should().Match<SelectListItem>(x =>
            x.Text == "Weekly" && x.Value == "3");
    }

    [Fact]
    public async Task EditNotificationCampusModel_OnGet_Sets_Search_Radius_Select_List()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .Build();
        //var id = notification.Id!.Value;

        var providerDataService = Substitute.For<IProviderDataService>();
        //providerDataService
        //    .GetNotification(id)
        //    .Returns(notification);

        var editNotificationCampusModel = new EditNotificationCampusModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await editNotificationCampusModel.OnGet();

        editNotificationCampusModel.SearchRadiusOptions.Should().NotBeNullOrEmpty();
        var options = editNotificationCampusModel.SearchRadiusOptions;

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
    public async Task EditNotificationCampusModel_OnGet_Sets_Skill_Area_Select_List()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var routes = new RouteBuilder()
            .BuildList()
            .ToList();

        //var notification = new NotificationBuilder()
        //    .Build();
        //var id = notification.Id!.Value;

        var providerDataService = Substitute.For<IProviderDataService>();
        //providerDataService
        //    .GetNotification(id)
        //    .Returns(notification);
        providerDataService
            .GetRoutes()
            .Returns(routes);

        var editNotificationCampusModel = new EditNotificationCampusModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await editNotificationCampusModel.OnGet();

        editNotificationCampusModel.Input.Should().NotBeNull();
        editNotificationCampusModel.Input!.SkillAreas.Should().NotBeNullOrEmpty();
        var skillAreas = editNotificationCampusModel.Input.SkillAreas;

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
    public async Task EditNotificationCampusModel_OnGet_Sets_Initial_Input_Values_With_Default_Search_Radius()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        //var notification = new NotificationBuilder()
        //    .WithSearchRadius(null)
        //    .Build();
        //var id = notification.Id!.Value;

        var providerDataService = Substitute.For<IProviderDataService>();
        //providerDataService
        //    .GetNotification(id)
        //    .Returns(notification);

        var editNotificationCampusModel = new EditNotificationCampusModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await editNotificationCampusModel.OnGet();

        editNotificationCampusModel.Input.Should().NotBeNull();
        //editNotificationCampusModel.Input!.NotificationId.Should().Be(id);
        editNotificationCampusModel.Input!.SelectedSearchRadius.Should().Be(settings.DefaultSearchRadius);
        editNotificationCampusModel.Input!.SelectedFrequency.Should().Be(NotificationFrequency.Immediately);
        editNotificationCampusModel.Input!.SelectedFrequency.Should().Be(NotificationFrequency.Immediately);
        editNotificationCampusModel.Input!.SelectedLocation.Should().Be(0);
    }

    [Fact]
    public async Task EditNotificationCampusModel_OnGet_Sets_Input_Selected_Values()
    {
        //const int searchRadius = 30;

        var settings = new SettingsBuilder().BuildProviderSettings();

        //var notification = new NotificationBuilder()
        //    .WithSearchRadius(searchRadius)
        //    .Build();
        //var id = notification.Id!.Value;

        var providerDataService = Substitute.For<IProviderDataService>();
        //providerDataService
        //    .GetNotification(id)
        //    .Returns(notification);

        var editNotificationCampusModel = new EditNotificationCampusModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await editNotificationCampusModel.OnGet();

        editNotificationCampusModel.Input.Should().NotBeNull();
        //editNotificationCampusModel.Input!.NotificationId.Should().Be(id);
        editNotificationCampusModel.Input!.SelectedSearchRadius.Should().Be(settings.DefaultSearchRadius);
        editNotificationCampusModel.Input!.SelectedFrequency.Should().Be(NotificationFrequency.Immediately);
    }

    [Fact]
    public async Task EditNotificationCampus_OnPost_Saves_To_Repository_And_Redirects()
    {
        const string testEmail = "test@test.com";
        //var notification = new NotificationBuilder()
        //    .Build();
        //var notificationId = notification.Id!.Value;

        var providerDataService = Substitute.For<IProviderDataService>();
        //providerDataService
        //    .GetNotification(notificationId)
        //    .Returns(notification);

        var editNotificationCampusModel = new EditNotificationCampusModelBuilder()
            .Build(providerDataService);

        editNotificationCampusModel.Input = new EditNotificationCampusModel.InputModel
        {
            Email = testEmail,
            SelectedSearchRadius = 30,
            SelectedFrequency = NotificationFrequency.Daily,
            SkillAreas = new[]
            {
                new SelectListItem("Value 1", "1", true)
            }
        };

        var result = await editNotificationCampusModel.OnPost();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Provider/Notifications");

        await providerDataService
            .Received(1)
            .SaveNotification(Arg.Any<Notification>(),
                PageContextBuilder.DefaultUkPrn);
    }
    
    [Fact]
    public async Task EditNotificationCampus_OnPost_Sets_TempData()
    {
        var notification = new NotificationBuilder()
            .Build();

        //var notificationId = notification.Id!.Value;

        var providerDataService = Substitute.For<IProviderDataService>();
        //providerDataService
        //    .GetNotification(notificationId)
        //    .Returns(notification);

        var editNotificationCampusModel = new EditNotificationCampusModelBuilder()
            .Build(providerDataService);

        editNotificationCampusModel.Input = new EditNotificationCampusModel.InputModel
        {
            Email = notification.Email
        };

        await editNotificationCampusModel.OnPost();

        editNotificationCampusModel.TempData.Should().NotBeNull();
        editNotificationCampusModel.TempData
            .Keys
            .Should()
            .Contain("VerificationEmail");

        editNotificationCampusModel.TempData
            .Peek("VerificationEmail")
            .Should()
            .Be(notification.Email);
    }

    [Fact]
    public async Task EditNotificationCampus_OnPostAddLocation_Saves_To_Repository_And_Redirects()
    {
        const string testEmail = "test@test.com";

        var providerDataService = Substitute.For<IProviderDataService>();

        var editNotificationCampusModel = new EditNotificationCampusModelBuilder()
            .Build(providerDataService);

        editNotificationCampusModel.Input = new EditNotificationCampusModel.InputModel
        {
            Email = testEmail,
            SelectedSearchRadius = 30,
            SelectedFrequency = NotificationFrequency.Daily,
            SkillAreas = new[]
            {
                new SelectListItem("Value 1", "1", true)
            }
        };

        var result = await editNotificationCampusModel.OnPostAddLocation();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Provider/Notifications");
        
        await providerDataService
            .Received(1)
            .SaveNotification(Arg.Any<Notification>(),
                PageContextBuilder.DefaultUkPrn);
    }
}
