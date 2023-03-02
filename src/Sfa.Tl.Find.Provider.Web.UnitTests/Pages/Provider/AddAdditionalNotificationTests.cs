using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages.Provider;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages.Provider;
public class AddAdditionalNotificationTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(AddAdditionalNotificationModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task AddAdditionalNotificationModel_OnGet_Sets_ExpectedProperties()
    {
        const int providerNotificationId = 1;

        var settings = new SettingsBuilder().BuildProviderSettings();

        var addAdditionalNotificationModel = new AddAdditionalNotificationModelBuilder()
            .Build(providerSettings: settings);

        await addAdditionalNotificationModel.OnGet(providerNotificationId);
    }

    [Fact]
    public async Task AddAdditionalNotificationLocationModel_OnGet_Sets_Provider_Locations_Select_List()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .Build();

        var availableLocations = new NotificationLocationNameBuilder()
            .BuildListOfAvailableLocations()
            .ToList();

        var notificationService = Substitute.For<INotificationService>();
        notificationService
            .GetNotification(notification.Id!.Value)
            .Returns(notification);
        notificationService
            .GetAvailableNotificationLocationPostcodes(notification.Id!.Value)
            .Returns(availableLocations);

        var addAdditionalNotificationModel = new AddAdditionalNotificationModelBuilder()
            .Build(notificationService,
                providerSettings: settings);

        await addAdditionalNotificationModel.OnGet(notification.Id.Value);

        addAdditionalNotificationModel.Locations.Should().NotBeNullOrEmpty();
        var options = addAdditionalNotificationModel.Locations;

        options!.Length.Should().Be(availableLocations.Count + 1);

        options[0].Should().Match<SelectListItem>(x =>
            x.Text == "Select a campus" && x.Value == "" && x.Selected);

        var i = 1;
        foreach (var location in availableLocations
                     .OrderBy(l => l.Name))
        {
            options[i].Should().Match<SelectListItem>(x =>
                x.Text == $"{location.Name.TruncateWithEllipsis(15).ToUpper()} [{location.Postcode}]" &&
                x.Value == location.LocationId.ToString() &&
                !x.Selected);
            i++;
        }
    }

    [Fact]
    public async Task AddAdditionalNotificationModel_OnGet_Sets_Provider_Locations_Select_List_For_Single_Location()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .Build();

        var availableLocations = new NotificationLocationNameBuilder()
            .BuildListOfAvailableLocations()
            .Take(1)
            .ToList();

        var notificationService = Substitute.For<INotificationService>();
        notificationService
            .GetNotification(notification.Id!.Value)
            .Returns(notification);
        notificationService
            .GetAvailableNotificationLocationPostcodes(notification.Id!.Value)
            .Returns(availableLocations);

        var addAdditionalNotificationModel = new AddAdditionalNotificationModelBuilder()
            .Build(notificationService,
                providerSettings: settings);

        await addAdditionalNotificationModel.OnGet(notification.Id.Value);

        addAdditionalNotificationModel.Locations.Should().NotBeNullOrEmpty();
        var options = addAdditionalNotificationModel.Locations;

        options!.Length.Should().Be(1);
        options.Single().Should().Match<SelectListItem>(x =>
            x.Text == $"{availableLocations.Single().Name.TruncateWithEllipsis(15).ToUpper()} [{availableLocations.Single().Postcode}]" &&
            x.Value == availableLocations.Single().LocationId.ToString() &&
            x.Selected);
    }

    [Fact]
    public async Task AddAdditionalNotificationModel_OnGet_Sets_FrequencyOptions_Select_List()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .Build();

        var notificationService = Substitute.For<INotificationService>();
        notificationService
            .GetNotification(notification.Id!.Value)
            .Returns(notification);

        var addAdditionalNotificationModel = new AddAdditionalNotificationModelBuilder()
            .Build(notificationService,
                providerSettings: settings);

        await addAdditionalNotificationModel.OnGet(notification.Id.Value);

        addAdditionalNotificationModel.FrequencyOptions.Should().NotBeNullOrEmpty();
        var options = addAdditionalNotificationModel.FrequencyOptions;

        options!.Length.Should().Be(3);
        options[0].Should().Match<SelectListItem>(x =>
            x.Text == "Immediately" && x.Value == "1");
        options[1].Should().Match<SelectListItem>(x =>
            x.Text == "Daily" && x.Value == "2");
        options[2].Should().Match<SelectListItem>(x =>
            x.Text == "Weekly" && x.Value == "3");
    }

    [Fact]
    public async Task AddAdditionalNotificationModel_OnGet_Sets_Search_Radius_Select_List()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .Build();

        var notificationService = Substitute.For<INotificationService>();
        notificationService
            .GetNotification(notification.Id!.Value)
            .Returns(notification);

        var addAdditionalNotificationModel = new AddAdditionalNotificationModelBuilder()
            .Build(notificationService,
                providerSettings: settings);

        await addAdditionalNotificationModel.OnGet(notification.Id.Value);

        addAdditionalNotificationModel.SearchRadiusOptions.Should().NotBeNullOrEmpty();
        var options = addAdditionalNotificationModel.SearchRadiusOptions;

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
    public async Task AddAdditionalNotificationModel_OnGet_Sets_Skill_Area_Select_List()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .Build();

        var routes = new RouteBuilder()
            .BuildList()
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        var notificationService = Substitute.For<INotificationService>();
        notificationService
            .GetNotification(notification.Id!.Value)
            .Returns(notification);
        providerDataService
            .GetRoutes()
            .Returns(routes);

        var addAdditionalNotificationModel = new AddAdditionalNotificationModelBuilder()
            .Build(notificationService, 
                providerDataService,
                providerSettings: settings);

        await addAdditionalNotificationModel.OnGet(notification.Id.Value);

        addAdditionalNotificationModel.Input.Should().NotBeNull();
        addAdditionalNotificationModel.Input!.SkillAreas.Should().NotBeNullOrEmpty();
        var skillAreas = addAdditionalNotificationModel.Input.SkillAreas;

        skillAreas!.Length.Should().Be(routes.Count);

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
    public async Task AddAdditionalNotificationModel_OnGet_Sets_Selected_Skill_Areas()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .Build();

        var routes = new RouteBuilder()
            .BuildList()
            .ToList();

        var notificationService = Substitute.For<INotificationService>();
        notificationService
            .GetNotification(notification.Id!.Value)
            .Returns(notification);
        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetRoutes()
            .Returns(routes);

        var addAdditionalNotificationModel = new AddAdditionalNotificationModelBuilder()
            .Build(notificationService, 
                providerDataService,
                providerSettings: settings);

        await addAdditionalNotificationModel.OnGet(notification.Id.Value);

        var skillAreas = addAdditionalNotificationModel.Input?.SkillAreas;
        skillAreas.Should().NotBeNull();
        var selectedSkillAreas = skillAreas!.Where(x => x.Selected).ToList();
        selectedSkillAreas.Count.Should().Be(notification.Routes.Count);

        foreach (var s in notification.Routes)
        {
            selectedSkillAreas.Should().Contain(x => x.Value == s.Id.ToString());
        }
    }

    [Fact]
    public async Task AddAdditionalNotificationModel_OnGet_Sets_Initial_Input_Values_With_Default_Search_Radius()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .WithSearchRadius(null)
            .Build();

        var providerDataService = Substitute.For<INotificationService>();
        providerDataService
            .GetNotification(notification.Id!.Value)
            .Returns(notification);

        var addAdditionalNotificationModel = new AddAdditionalNotificationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await addAdditionalNotificationModel.OnGet(notification.Id.Value);

        addAdditionalNotificationModel.Input.Should().NotBeNull();
        addAdditionalNotificationModel.Input!.ProviderNotificationId.Should().Be(notification.Id.Value);
        addAdditionalNotificationModel.Input!.SelectedSearchRadius.Should().Be(settings.DefaultNotificationSearchRadius);
        addAdditionalNotificationModel.Input!.SelectedFrequency.Should().Be(notification.Frequency);
        addAdditionalNotificationModel.Input!.SelectedLocation.Should().Be(null);
    }

    [Fact]
    public async Task AddAdditionalNotificationModel_OnGet_Sets_Input_Selected_Values()
    {
        const int searchRadius = 30;

        var settings = new SettingsBuilder().BuildProviderSettings();

        var notification = new NotificationBuilder()
            .WithSearchRadius(searchRadius)
            .Build();

        var providerDataService = Substitute.For<INotificationService>();
        providerDataService
            .GetNotification(notification.Id!.Value)
            .Returns(notification);

        var addAdditionalNotificationModel = new AddAdditionalNotificationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await addAdditionalNotificationModel.OnGet(notification.Id.Value);

        addAdditionalNotificationModel.Input.Should().NotBeNull();
        addAdditionalNotificationModel.Input!.ProviderNotificationId.Should().Be(notification.Id!.Value);
        addAdditionalNotificationModel.Input!.SelectedSearchRadius.Should().Be(settings.DefaultNotificationSearchRadius);
        addAdditionalNotificationModel.Input!.SelectedFrequency.Should().Be(notification.Frequency);
    }

    [Fact]
    public async Task AddAdditionalNotificationModel_OnGet_Redirects_To_EditNotification_If_Provider_Notification_Not_Found()
    {
        const int providerNotificationId = 99;
        var providerDataService = Substitute.For<INotificationService>();
        providerDataService
            .GetNotification(providerNotificationId)
            .Returns(null as Notification);

        var addAdditionalNotificationModel = new AddAdditionalNotificationModelBuilder()
            .Build(providerDataService);

        var result = await addAdditionalNotificationModel.OnGet(providerNotificationId);

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Provider/Notifications");
        redirectResult.RouteValues.Should().Contain(x =>
            x.Key == "id" &&
            x.Value != null &&
            x.Value.ToString() == $"{providerNotificationId}");
    }

    [Fact]
    public async Task AddAdditionalNotificationModel_OnPost_Saves_To_Repository_And_Redirects()
    {
        const int providerNotificationId = 1;

        var notificationService = Substitute.For<INotificationService>();

        var addAdditionalNotificationModel = new AddAdditionalNotificationModelBuilder()
            .Build(notificationService);

        addAdditionalNotificationModel.Input = new AddAdditionalNotificationModel.InputModel
        {
            ProviderNotificationId = providerNotificationId,
            SelectedSearchRadius = 30,
            SelectedFrequency = NotificationFrequency.Daily,
            SkillAreas = new[]
            {
                new SelectListItem("Value 1", "1", true)
            }
        };

        var result = await addAdditionalNotificationModel.OnPost();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Provider/Notifications");

        await notificationService
            .Received(1)
            .CreateNotificationLocation(Arg.Any<Notification>(), providerNotificationId);
    }

    [Fact]
    public async Task AddAdditionalNotificationModel_OnPost_Sets_TempData()
    {
        var notification = new NotificationBuilder()
            .Build();

        var notificationService = Substitute.For<INotificationService>();
        notificationService
            .GetNotification(notification.Id!.Value)
            .Returns(notification);

        var addAdditionalNotificationModel = new AddAdditionalNotificationModelBuilder()
            .Build(notificationService);

        addAdditionalNotificationModel.Input = new AddAdditionalNotificationModel.InputModel
        {
            ProviderNotificationId = notification.Id.Value,
            SelectedFrequency = NotificationFrequency.Immediately,
            SelectedLocation = null,
            SelectedSearchRadius = null,
            SkillAreas = Array.Empty<SelectListItem>()
        };

        await addAdditionalNotificationModel.OnPost();

        addAdditionalNotificationModel.TempData.Should().NotBeNull();
        addAdditionalNotificationModel.TempData
            .Keys
            .Should()
            .Contain("VerificationEmail");

        addAdditionalNotificationModel.TempData
            .Peek("VerificationEmail")
            .Should()
            .Be(notification.Email);
    }

    [Fact]
    public async Task AddAdditionalNotification_OnPostAddLocation_Saves_To_Repository_And_Redirects()
    {
        var notification = new NotificationBuilder()
            .Build();

        var providerNotificationId = notification.Id!.Value;

        var notificationService = Substitute.For<INotificationService>();
        
        var addAdditionalNotificationModel = new AddAdditionalNotificationModelBuilder()
            .Build(notificationService);

        addAdditionalNotificationModel.Input = new AddAdditionalNotificationModel.InputModel
        {
            ProviderNotificationId = providerNotificationId,
            SelectedSearchRadius = 30,
            SelectedFrequency = NotificationFrequency.Daily,
            SkillAreas = new[]
            {
                new SelectListItem("Value 1", "1", true)
            }
        };

        var result = await addAdditionalNotificationModel.OnPostAddLocation();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Provider/AddAdditionalNotification");
        redirectResult.RouteValues.Should().Contain(x =>
            x.Key == "id" &&
            x.Value != null &&
            x.Value.ToString() == providerNotificationId.ToString());

        await notificationService
            .Received(1)
            .CreateNotificationLocation(Arg.Any<Notification>(), providerNotificationId);
    }
}
