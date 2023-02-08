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
public class AddNotificationTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(AddNotificationModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task AddNotificationModel_OnGet_Sets_ExpectedProperties()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var addNotificationModel = new AddNotificationModelBuilder()
            .Build(providerSettings: settings);

        await addNotificationModel.OnGet();

        //addNotificationModel.DefaultSearchRadius.Should().Be(settings.DefaultSearchRadius);
    }

    [Fact]
    public async Task AddNotificationModel_OnGet_Sets_Locations_Select_List()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var locations = new LocationPostcodeBuilder()
            .BuildList()
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetLocationPostcodes(PageContextBuilder.DefaultUkPrn)
            .Returns(locations);

        var addNotificationModel = new AddNotificationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await addNotificationModel.OnGet();

        addNotificationModel.Locations.Should().NotBeNullOrEmpty();
        var options = addNotificationModel.Locations;

        options!.Length.Should().Be(locations.Count + 1);

        options[0].Should().Match<SelectListItem>(x =>
            x.Text == "All" && x.Value == "0" && x.Selected);

        var orderedLocations = locations
            .OrderBy(r => r.Name)
            .ToArray();

        for (var i = 1; i < options.Length; i++)
        {
            options[i].Should().Match<SelectListItem>(x =>
                x.Text == $"{orderedLocations[i -1].Name.TruncateWithEllipsis(15).ToUpper()} [{orderedLocations[i - 1].Postcode}]" &&
                x.Value == orderedLocations[i - 1].Id.ToString());
        }
    }

    [Fact]
    public async Task AddNotificationModel_OnGet_Does_Not_Set_Locations_Select_List_When_Only_One_Location()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var locations = new LocationPostcodeBuilder()
            .BuildList()
            .Take(1)
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetLocationPostcodes(PageContextBuilder.DefaultUkPrn)
            .Returns(locations);

        var addNotificationModel = new AddNotificationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await addNotificationModel.OnGet();

        addNotificationModel.Locations.Should().BeNullOrEmpty();
        addNotificationModel.Input.Should().NotBeNull();
        addNotificationModel.Input!.SelectedLocation.
            Should().Be(locations.Single().Id);
    }

    [Fact]
    public async Task AddNotificationModel_OnGet_Sets_FrequencyOptions_Select_List()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var providerDataService = Substitute.For<IProviderDataService>();

        var addNotificationModel = new AddNotificationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await addNotificationModel.OnGet();

        addNotificationModel.SearchRadiusOptions.Should().NotBeNullOrEmpty();
        var options = addNotificationModel.FrequencyOptions;

        options!.Length.Should().Be(3);
        options[0].Should().Match<SelectListItem>(x =>
            x.Text == "Immediately" && x.Value == "1");
        options[1].Should().Match<SelectListItem>(x =>
            x.Text == "Daily" && x.Value == "2");
        options[2].Should().Match<SelectListItem>(x =>
            x.Text == "Weekly" && x.Value == "3");
    }

    [Fact]
    public async Task AddNotificationModel_OnGet_Sets_Search_Radius_Select_List()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var providerDataService = Substitute.For<IProviderDataService>();

        var addNotificationModel = new AddNotificationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await addNotificationModel.OnGet();

        addNotificationModel.SearchRadiusOptions.Should().NotBeNullOrEmpty();
        var options = addNotificationModel.SearchRadiusOptions;

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
    public async Task AddNotificationModel_OnGet_Sets_Skill_Area_Select_List()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var routes = new RouteBuilder()
            .BuildList()
            .ToList();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetRoutes()
            .Returns(routes);

        var addNotificationModel = new AddNotificationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await addNotificationModel.OnGet();

        addNotificationModel.Input.Should().NotBeNull();
        addNotificationModel.Input!.SkillAreas.Should().NotBeNullOrEmpty();
        var skillAreas = addNotificationModel.Input.SkillAreas;

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
    public async Task AddNotificationModel_OnGet_Sets_Initial_Input_Values_With_Default_Search_Radius()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var providerDataService = Substitute.For<IProviderDataService>();

        var addNotificationModel = new AddNotificationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await addNotificationModel.OnGet();

        addNotificationModel.Input.Should().NotBeNull();
        addNotificationModel.Input!.SelectedSearchRadius.Should().Be(settings.DefaultNotificationSearchRadius);
        addNotificationModel.Input!.SelectedFrequency.Should().Be(NotificationFrequency.Immediately);
        addNotificationModel.Input!.SelectedLocation.Should().Be(0);
    }

    [Fact]
    public async Task AddNotificationModel_OnGet_Sets_Input_Selected_Values()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var providerDataService = Substitute.For<IProviderDataService>();

        var addNotificationModel = new AddNotificationModelBuilder()
            .Build(providerDataService,
                providerSettings: settings);

        await addNotificationModel.OnGet();

        addNotificationModel.Input.Should().NotBeNull();
        addNotificationModel.Input!.SelectedSearchRadius.Should().Be(settings.DefaultNotificationSearchRadius);
        addNotificationModel.Input!.SelectedFrequency.Should().Be(NotificationFrequency.Immediately);
    }

    [Fact]
    public async Task AddNotification_OnPost_Saves_To_Repository_And_Redirects()
    {
        const string testEmail = "test@test.com";

        var providerDataService = Substitute.For<IProviderDataService>();

        var addNotificationModel = new AddNotificationModelBuilder()
            .Build(providerDataService);

        addNotificationModel.Input = new AddNotificationModel.InputModel
        {
            Email = testEmail,
            SelectedSearchRadius = 30,
            SelectedFrequency = NotificationFrequency.Daily,
            SkillAreas = new[]
            {
                new SelectListItem("Value 1", "1", true)
            }
        };

        var result = await addNotificationModel.OnPost();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Provider/Notifications");

        await providerDataService
            .Received(1)
            .SaveNotification(Arg.Any<Notification>(),
                PageContextBuilder.DefaultUkPrn);
    }
    
    [Fact]
    public async Task AddNotification_OnPost_Sets_TempData()
    {
        var notification = new NotificationBuilder()
            .Build();

        var providerDataService = Substitute.For<IProviderDataService>();

        var addNotificationModel = new AddNotificationModelBuilder()
            .Build(providerDataService);

        addNotificationModel.Input = new AddNotificationModel.InputModel
        {
            Email = notification.Email
        };

        await addNotificationModel.OnPost();

        addNotificationModel.TempData.Should().NotBeNull();
        addNotificationModel.TempData
            .Keys
            .Should()
            .Contain("VerificationEmail");

        addNotificationModel.TempData
            .Peek("VerificationEmail")
            .Should()
            .Be(notification.Email);
    }

    [Fact]
    public async Task AddNotification_OnPostAddLocation_Saves_To_Repository_And_Redirects()
    {
        const int newId = 1;
        const string testEmail = "test@test.com";

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .SaveNotification(Arg.Any<Notification>(),
                PageContextBuilder.DefaultUkPrn)
            .Returns(newId);

        var addNotificationModel = new AddNotificationModelBuilder()
            .Build(providerDataService);

        addNotificationModel.Input = new AddNotificationModel.InputModel
        {
            Email = testEmail,
            SelectedSearchRadius = 30,
            SelectedFrequency = NotificationFrequency.Daily,
            SkillAreas = new[]
            {
                new SelectListItem("Value 1", "1", true)
            }
        };

        var result = await addNotificationModel.OnPostAddLocation();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Provider/AddNotificationLocation");
        redirectResult.RouteValues.Should().Contain(x =>
            x.Key == "id" &&
            x.Value != null &&
            x.Value.ToString() == newId.ToString());

        await providerDataService
            .Received(1)
            .SaveNotification(Arg.Any<Notification>(),
                PageContextBuilder.DefaultUkPrn);
    }
}
