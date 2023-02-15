using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages.Provider;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages.Provider;

public class SearchFilterDetailsTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(SearchFilterDetailsModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task SearchFilterDetailsModel_OnGet_Sets_ExpectedProperties()
    {
        const int id = 999;
        var settings = new SettingsBuilder().BuildProviderSettings();

        var searchFilterDetailsModel = new SearchFilterDetailsModelBuilder()
            .Build(providerSettings: settings);

        await searchFilterDetailsModel.OnGet(id);

        searchFilterDetailsModel.DefaultSearchRadius.Should().Be(settings.DefaultSearchRadius);
    }

    [Fact]
    public async Task SearchFilterDetailsModel_OnGet_Sets_Search_Filter_From_Service()
    {
        var searchFilter = new SearchFilterBuilder()
            .Build();
        var id = searchFilter.LocationId;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetSearchFilter(id)
            .Returns(searchFilter);

        var detailsModel = new SearchFilterDetailsModelBuilder()
            .Build(providerDataService);

        await detailsModel.OnGet(id);

        detailsModel.SearchFilter
            .Should()
            .BeEquivalentTo(searchFilter);
    }

    [Fact]
    public async Task SearchFilterDetailsModel_OnGet_Sets_Search_Radius_Select_List()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var searchFilter = new SearchFilterBuilder()
            .Build();
        var id = searchFilter.LocationId;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetSearchFilter(id)
            .Returns(searchFilter);

        var searchFilterDetailsModel = new SearchFilterDetailsModelBuilder()
            .Build(providerDataService, settings);

        await searchFilterDetailsModel.OnGet(id);

        searchFilterDetailsModel.SearchRadiusOptions.Should().NotBeNullOrEmpty();
        var options = searchFilterDetailsModel.SearchRadiusOptions;

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
    public async Task SearchFilterDetailsModel_OnGet_Sets_Skill_Area_Select_List()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var routes = new RouteBuilder()
            .BuildList()
            .ToList();

        var searchFilter = new SearchFilterBuilder()
            .Build();
        var id = searchFilter.LocationId;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetSearchFilter(id)
            .Returns(searchFilter);
        providerDataService
            .GetRoutes()
            .Returns(routes);

        var searchFilterDetailsModel = new SearchFilterDetailsModelBuilder()
            .Build(providerDataService, settings);

        await searchFilterDetailsModel.OnGet(id);

        searchFilterDetailsModel.Input.Should().NotBeNull();
        searchFilterDetailsModel.Input!.SkillAreas.Should().NotBeNullOrEmpty();
        var skillAreas = searchFilterDetailsModel.Input.SkillAreas;

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
    public async Task SearchFilterDetailsModel_OnGet_Sets_Default_SelectedSearchRadius()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var searchFilter = new SearchFilterBuilder()
            .WithSearchRadius(null)
            .Build();
        var id = searchFilter.LocationId;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetSearchFilter(id)
            .Returns(searchFilter);

        var searchFilterDetailsModel = new SearchFilterDetailsModelBuilder()
            .Build(providerDataService, settings);

        await searchFilterDetailsModel.OnGet(id);

        searchFilterDetailsModel.Input.Should().NotBeNull();
        searchFilterDetailsModel.Input!.SelectedSearchRadius.Should().Be(settings.DefaultSearchRadius);
    }

    [Fact]
    public async Task SearchFilterDetailsModel_OnGet_Sets_Input_SelectedSearchRadius()
    {
        const int searchRadius = 30;

        var settings = new SettingsBuilder().BuildProviderSettings();

        var searchFilter = new SearchFilterBuilder()
            .WithSearchRadius(searchRadius)
            .Build();
        var id = searchFilter.LocationId;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetSearchFilter(id)
            .Returns(searchFilter);

        var searchFilterDetailsModel = new SearchFilterDetailsModelBuilder()
            .Build(providerDataService, settings);

        await searchFilterDetailsModel.OnGet(id);

        searchFilterDetailsModel.Input.Should().NotBeNull();
        searchFilterDetailsModel.Input!.LocationId.Should().Be(id);
        searchFilterDetailsModel.Input!.SelectedSearchRadius.Should().Be(searchRadius);
    }

    [Fact]
    public async Task SearchFilterDetailsModel_OnGet_Redirects_To_404_If_Filter_Not_Found()
    {
        const int id = 999;

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService
            .GetSearchFilter(id)
            .Returns(null as SearchFilter);

        var detailsModel = new SearchFilterDetailsModelBuilder()
            .Build(providerDataService);

        var result = await detailsModel.OnGet(id);

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Error/404");
    }

    [Fact]
    public async Task SearchFilterDetailsModel_OnPost_Calls_Service()
    {
        var providerDataService = Substitute.For<IProviderDataService>();

        var detailsModel = new SearchFilterDetailsModelBuilder()
            .Build(providerDataService);

        var id = 10;
        var searchRadius = 30;

        var skillAreas = new SelectListItem[]
        {
            new("Agriculture, environment and animal care",
                "1", 
                false),
            new("Digital and IT", "6", true),
            new("Health and science", "10", true)
        };
            
        detailsModel.Input = new SearchFilterDetailsModel.InputModel
        {
            LocationId = id,
            SelectedSearchRadius = searchRadius,
            SkillAreas = skillAreas
        };

        await detailsModel.OnPost();
        
        await providerDataService
            .Received(1)
            .SaveSearchFilter(Arg.Is<SearchFilter>(
                s => s.LocationId == id &&
                     s.SearchRadius == searchRadius));

        await providerDataService
            .Received(1)
            .SaveSearchFilter(Arg.Is<SearchFilter>(
                s => s.Routes.Count == 2 &&
                     s.Routes.Any(r => r.Id == 6) &&
                     s.Routes.Any(r => r.Id == 10)));
    }

    [Fact]
    public async Task SearchFilterDetailsModel_OnPost_Redirects_To_Search_Filters_List_Page()
    {
        var detailsModel = new SearchFilterDetailsModelBuilder()
            .Build();

        var id = 10;
        var searchRadius = 30;

        detailsModel.Input = new SearchFilterDetailsModel.InputModel
        {
            LocationId = id,
            SelectedSearchRadius = searchRadius,
            SkillAreas = Array.Empty<SelectListItem>()
        };

        var result = await detailsModel.OnPost();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be("/Provider/SearchFilters");
    }
}