using Microsoft.AspNetCore.Mvc.Rendering;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Web.Extensions;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Extensions;
public class SelectListHelperExtensionsTests
{
    [Fact]
    public void GetSelectedSkillAreas_Returns_Expected_Routes()
    {
        var selectedList = new[]
        {
            new SelectListItem("Value 1", "1", true),
            new SelectListItem("Value 3", "3", true)
        };

        var selectedSkillAreas = SelectListHelperExtensions.GetSelectedSkillAreas(selectedList);

        selectedSkillAreas.Should().NotBeNullOrEmpty();
        selectedSkillAreas.Count.Should().Be(selectedList.Length);
        selectedSkillAreas[0].Should().Match<Route>(r => r.Id == 1);
        selectedSkillAreas[1].Should().Match<Route>(r => r.Id == 3);
    }

    [Fact]
    public void LoadFrequencyOptions_Returns_Expected_FrequencyOptions_Select_List()
    {
        var options = SelectListHelperExtensions.LoadFrequencyOptions(NotificationFrequency.Daily);

        options.Length.Should().Be(3);

        options[0].Should().Match<SelectListItem>(x =>
            x.Text == "Immediately" && x.Value == "1");
        options[1].Should().Match<SelectListItem>(x =>
            x.Text == "Daily" && x.Value == "2"
            && x.Selected);
        options[2].Should().Match<SelectListItem>(x =>
            x.Text == "Weekly" && x.Value == "3");
    }

    [Fact]
    public void LoadProviderLocationOptions_Returns_Expected_Select_List()
    {
        var availableLocations = new NotificationLocationNameBuilder()
            .BuildListOfAvailableLocations()
            .ToList();

        var options = SelectListHelperExtensions
            .LoadProviderLocationOptions(
                availableLocations,
                null);

        options.Length.Should().Be(availableLocations.Count + 1);

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
    public void LoadProviderLocationOptions_Returns_Expected_Select_List_With_Selected_Item()
    {
        var availableLocations = new NotificationLocationNameBuilder()
            .BuildListOfAvailableLocations()
            .ToList();

        var selectedLocation = availableLocations
            .Skip(1)
            .First()
            .Id;

        var options = SelectListHelperExtensions
            .LoadProviderLocationOptions(
                availableLocations,
                selectedLocation);

        options.Length.Should().Be(availableLocations.Count + 1);

        options[0].Should().Match<SelectListItem>(x =>
            x.Text == "Select a campus" && x.Value == "" && x.Selected);

        var i = 1;
        foreach (var location in availableLocations
                     .OrderBy(l => l.Name))
        {
            options[i].Should().Match<SelectListItem>(x =>
                    x.Text == $"{location.Name.TruncateWithEllipsis(15).ToUpper()} [{location.Postcode}]" &&
                    x.Value == location.LocationId.ToString() &&
                    x.Selected == (x.Value == selectedLocation.ToString()));
            i++;
        }
    }

    [Fact]
    public void LoadProviderLocationOptionsWithAllOption_Returns_Expected_Select_List()
    {
        var locations = new LocationPostcodeBuilder()
            .BuildList()
            .ToList();

        var options = SelectListHelperExtensions
            .LoadProviderLocationOptionsWithAllOption(
                locations,
                null);

        options.Length.Should().Be(locations.Count + 1);

        options[0].Should().Match<SelectListItem>(x =>
            x.Text == "All" && x.Value == "0" && x.Selected);

        var i = 1;
        foreach (var location in locations
                     .OrderBy(l => l.Name))
        {
            options[i].Should().Match<SelectListItem>(x =>
                x.Text == $"{location.Name.TruncateWithEllipsis(15).ToUpper()} [{location.Postcode}]" &&
                x.Value == location.Id.ToString() &&
                !x.Selected);
            i++;
        }
    }

    [Fact]
    public void LoadProviderLocationOptionsWithAllOption_Returns_Expected_Select_List_With_Selected_Item()
    {
        var locations = new LocationPostcodeBuilder()
            .BuildList()
            .ToList();

        var selectedLocation = locations
            .Skip(1)
            .First()
            .Id;

        var options = SelectListHelperExtensions
            .LoadProviderLocationOptionsWithAllOption(
                locations,
                selectedLocation);

        options.Length.Should().Be(locations.Count + 1);

        options[0].Should().Match<SelectListItem>(x =>
            x.Text == "All" && x.Value == "0" && !x.Selected);

        var i = 1;
        foreach (var location in locations
                     .OrderBy(l => l.Name))
        {
            options[i].Should().Match<SelectListItem>(x =>
                x.Text == $"{location.Name.TruncateWithEllipsis(15).ToUpper()} [{location.Postcode}]" &&
                x.Value == location.Id.ToString() &&
                x.Selected == (x.Value == selectedLocation.ToString()));
            i++;
        }
    }

    [Fact]
    public void LoadSearchRadiusOptions_Returns_Expected_Select_List()
    {
        var options = SelectListHelperExtensions.LoadSearchRadiusOptions(10);

        options.Length.Should().Be(6);
        options[0].Should().Match<SelectListItem>(x =>
            x.Text == "5 miles" && x.Value == "5" && !x.Selected);
        options[1].Should().Match<SelectListItem>(x =>
            x.Text == "10 miles" && x.Value == "10" && x.Selected);
        options[2].Should().Match<SelectListItem>(x =>
            x.Text == "20 miles" && x.Value == "20" && !x.Selected);
        options[3].Should().Match<SelectListItem>(x =>
            x.Text == "30 miles" && x.Value == "30" && !x.Selected);
        options[4].Should().Match<SelectListItem>(x =>
            x.Text == "40 miles" && x.Value == "40" && !x.Selected);
        options[5].Should().Match<SelectListItem>(x =>
            x.Text == "50 miles" && x.Value == "50" && !x.Selected);
    }

    [Fact]
    public void LoadSkillAreaOptions_Returns_Expected_Select_List()
    {
        var routes = new RouteBuilder()
            .BuildList()
            .ToList();

        var skillAreas = SelectListHelperExtensions
            .LoadSkillAreaOptions(
                routes,
                new List<Route>());

        skillAreas.Length.Should().Be(routes.Count);

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
    public void LoadSkillAreaOptions_Sets_Selected_Skill_Areas()
    {
        var notification = new NotificationBuilder()
            .Build();

        var routes = new RouteBuilder()
             .BuildList()
             .ToList();

        var skillAreas = SelectListHelperExtensions
            .LoadSkillAreaOptions(
                routes,
                notification.Routes);

        skillAreas.Should().NotBeNull();
        var selectedSkillAreas = skillAreas.Where(x => x.Selected).ToList();
        selectedSkillAreas.Count.Should().Be(notification.Routes.Count);

        foreach (var s in notification.Routes)
        {
            selectedSkillAreas.Should().Contain(x => x.Value == s.Id.ToString());
        }
    }
}
