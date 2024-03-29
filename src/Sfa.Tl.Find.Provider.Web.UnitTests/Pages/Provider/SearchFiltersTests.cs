﻿using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages.Provider;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages.Provider;
public  class SearchFiltersTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(SearchFiltersModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task SearchFiltersModel_OnGet_Sets_ExpectedProperties()
    {
        const int expectedDefaultSearchRadius = 20;
        var settings = new SettingsBuilder().BuildProviderSettings();

        var searchFiltersModel = new SearchFiltersModelBuilder()
            .Build(providerSettings: settings);

        await searchFiltersModel.OnGet();

        searchFiltersModel.DefaultSearchRadius.Should().Be(expectedDefaultSearchRadius);
    }

    [Fact]
    public async Task SearchFiltersModel_OnGet_Populates_EmployerInterest_List_For_Administrator()
    {
        var searchFilterList = new SearchFilterBuilder()
            .BuildList()
            .ToList();

        var searchFilterService = Substitute.For<ISearchFilterService>();
        searchFilterService
            .GetSearchFilterSummaryList(PageContextBuilder.DefaultUkPrn)
            .Returns(searchFilterList);

        var searchFiltersModel = new SearchFiltersModelBuilder()
            .Build(searchFilterService);

        await searchFiltersModel.OnGet();
        
        searchFiltersModel
            .SearchFilterList
            .Should()
            .BeEquivalentTo(searchFilterList);
    }
}
