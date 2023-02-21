using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;

namespace Sfa.Tl.Find.Provider.Application.Services;
public class SearchFilterService : ISearchFilterService
{
    private readonly ISearchFilterRepository _searchFilterRepository;
    private readonly ILogger<SearchFilterService> _logger;

    public SearchFilterService(
        ISearchFilterRepository searchFilterRepository,
        ILogger<SearchFilterService> logger)
    {
        _searchFilterRepository = searchFilterRepository ?? throw new ArgumentNullException(nameof(searchFilterRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<SearchFilter>> GetSearchFilterSummaryList(long ukPrn)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Getting search filters");
        }

        return await _searchFilterRepository
            .GetSearchFilterSummaryList(ukPrn);
    }

    public async Task<SearchFilter> GetSearchFilter(int locationId)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Getting search filter for location {locationId}", locationId);
        }

        return await _searchFilterRepository
            .GetSearchFilter(locationId);
    }

    public async Task SaveSearchFilter(SearchFilter searchFilter)
    {
        await _searchFilterRepository.Save(searchFilter);
    }
}
