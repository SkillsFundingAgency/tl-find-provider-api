using System.Data;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Data;
public class SearchFilterRepository : ISearchFilterRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly IDynamicParametersWrapper _dynamicParametersWrapper;
    private readonly ILogger<SearchFilterRepository> _logger;
    private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;

    public SearchFilterRepository(
        IDbContextWrapper dbContextWrapper,
        IDynamicParametersWrapper dynamicParametersWrapper,
        IReadOnlyPolicyRegistry<string> policyRegistry,
        ILogger<SearchFilterRepository> logger)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        _dynamicParametersWrapper = dynamicParametersWrapper ?? throw new ArgumentNullException(nameof(dynamicParametersWrapper));
        _policyRegistry = policyRegistry ?? throw new ArgumentNullException(nameof(policyRegistry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<SearchFilter>> GetSearchFilters(
        long ukPrn,
        bool includeAdditionalData)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            ukPrn,
            includeAdditionalData
        });

        var searchFilters = new Dictionary<int, SearchFilter>();

        await _dbContextWrapper
            .QueryAsync<SearchFilterDto, RouteDto, SearchFilter>(
                connection,
                "GetSearchFilters",
                (e, r) =>
                {
                    if (!searchFilters.TryGetValue(e.LocationId, out var searchFilter))
                    {
                        searchFilters.Add(e.LocationId,
                            searchFilter = new SearchFilter
                            {
                                Id = e.Id,
                                LocationId = e.LocationId,
                                LocationName = e.LocationName,
                                Postcode = e.Postcode,
                                SearchRadius = e.SearchRadius,
                                Routes = new List<Route>()
                            });
                    }

                    if (r is not null)
                    {
                        searchFilter.Routes.Add(
                            new Route
                            {
                                Id = r.RouteId,
                                Name = r.RouteName
                            });
                    }

                    return searchFilter;
                },
                _dynamicParametersWrapper.DynamicParameters,
                splitOn: "Id, RouteId",
                commandType: CommandType.StoredProcedure);

        return searchFilters.Values;
    }

    public async Task<SearchFilter> GetSearchFilter(
        int locationId)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            locationId
        });

        SearchFilter searchFilter = null;

        await _dbContextWrapper
            .QueryAsync<SearchFilterDto, RouteDto, SearchFilter>(
                connection,
                "GetSearchFilterDetail",
                (e, r) =>
                {
                    searchFilter ??= new SearchFilter
                    {
                        Id = e.Id,
                        LocationId = e.LocationId,
                        LocationName = e.LocationName,
                        Postcode = e.Postcode,
                        SearchRadius = e.SearchRadius,
                        Routes = new List<Route>()
                    };

                    if (r is not null)
                    {
                        searchFilter.Routes.Add(
                            new Route
                            {
                                Id = r.RouteId,
                                Name = r.RouteName
                            });
                    }

                    return searchFilter;
                },
                _dynamicParametersWrapper.DynamicParameters,
                splitOn: "Id, RouteId",
                commandType: CommandType.StoredProcedure);

        return searchFilter;
    }
}
