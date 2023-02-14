using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Services;
public class SearchFilterServiceTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(SearchFilterService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(SearchFilterService)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task GetSearchFilters_Returns_Expected_List()
    {
        const long ukPrn = 12345678;

        var searchFilters = new SearchFilterBuilder()
            .BuildList()
            .ToList();

        var searchFilterRepository = Substitute.For<ISearchFilterRepository>();
        searchFilterRepository.GetSearchFilterSummaryList(ukPrn, Arg.Any<bool>())
            .Returns(searchFilters);

        var service = new SearchFilterServiceBuilder()
            .Build(searchFilterRepository: searchFilterRepository);

        var results = (await service.GetSearchFilterSummaryList(ukPrn)).ToList();
        results.Should().BeEquivalentTo(searchFilters);

        await searchFilterRepository
            .Received(1)
            .GetSearchFilterSummaryList(ukPrn, true);
    }

    [Fact]
    public async Task GetSearchFilter_Returns_Expected_Item()
    {
        const int id = 1;

        var searchFilter = new SearchFilterBuilder()
            .Build();

        var searchFilterRepository = Substitute.For<ISearchFilterRepository>();
        searchFilterRepository.GetSearchFilter(id)
            .Returns(searchFilter);

        var service = new SearchFilterServiceBuilder()
            .Build(searchFilterRepository: searchFilterRepository);

        var result = await service.GetSearchFilter(id);
        result.Should().BeEquivalentTo(searchFilter);

        await searchFilterRepository
            .Received(1)
            .GetSearchFilter(id);
    }

    [Fact]
    public async Task SaveSearchFilter_Calls_Repository()
    {
        var searchFilter = new SearchFilterBuilder()
            .Build();

        var searchFilterRepository = Substitute.For<ISearchFilterRepository>();

        var service = new SearchFilterServiceBuilder()
            .Build(searchFilterRepository: searchFilterRepository);

        await service.SaveSearchFilter(searchFilter);

        await searchFilterRepository
            .Received(1)
            .Save(searchFilter);
    }
}
