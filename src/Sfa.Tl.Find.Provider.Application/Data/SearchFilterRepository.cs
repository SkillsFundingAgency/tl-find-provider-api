﻿using System.Data;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Data;
public class SearchFilterRepository : ISearchFilterRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly IDynamicParametersWrapper _dynamicParametersWrapper;
    private readonly ILogger<SearchFilterRepository> _logger;

    public SearchFilterRepository(
        IDbContextWrapper dbContextWrapper,
        IDynamicParametersWrapper dynamicParametersWrapper,
        ILogger<SearchFilterRepository> logger)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        _dynamicParametersWrapper = dynamicParametersWrapper ?? throw new ArgumentNullException(nameof(dynamicParametersWrapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<SearchFilter>> GetSearchFilterSummaryList(
        long ukPrn)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            ukPrn
        });

        var searchFilters = new Dictionary<int, SearchFilter>();

        await _dbContextWrapper
            .QueryAsync<SearchFilterDto, RouteDto, SearchFilter>(
                connection,
                "GetSearchFilterSummary",
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

    public async Task Save(SearchFilter searchFilter)
    {
        try
        {
            using var connection = _dbContextWrapper.CreateConnection();

            _dynamicParametersWrapper.CreateParameters(new
            {
                locationId = searchFilter.LocationId,
                searchRadius = searchFilter.SearchRadius,
                routeIds = searchFilter.Routes
                    .Select(r => r.Id)
                    .AsTableValuedParameter("dbo.IdListTableType")
            });

            var result = await _dbContextWrapper.ExecuteAsync(
                connection,
                "CreateOrUpdateSearchFilter",
                _dynamicParametersWrapper.DynamicParameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when saving employer interest");
            throw;
        }
    }
}
