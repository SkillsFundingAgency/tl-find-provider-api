using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Data;

public class ProviderRepository : IProviderRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILogger<ProviderRepository> _logger;
    private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;

    public ProviderRepository(
        IDbContextWrapper dbContextWrapper,
        IDateTimeService dateTimeService,
        IReadOnlyPolicyRegistry<string> policyRegistry,
        ILogger<ProviderRepository> logger)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
        _policyRegistry = policyRegistry ?? throw new ArgumentNullException(nameof(policyRegistry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HasAny(bool isAdditionalData = false)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        var result = await _dbContextWrapper.ExecuteScalarAsync<int>(
            connection,
            "SELECT COUNT(1) " +
            "WHERE EXISTS (SELECT 1 FROM dbo.Provider WHERE IsAdditionalData = @isAdditionalData)",
            new { isAdditionalData = isAdditionalData ? 1 : 0 }
        );

        return result != 0;
    }

    public async Task Save(IList<Models.Provider> providers, bool isAdditionalData = false)
    {
        try
        {
            var locationData = new List<LocationDto>();
            var locationQualificationData = new List<LocationQualificationDto>();

            foreach (var provider in providers)
            {
                foreach (var location in provider.Locations)
                {
                    locationData.Add(location.MapToLocationDto(provider.UkPrn));
                    locationQualificationData.AddRange(
                        location
                            .DeliveryYears
                            .MapToLocationQualificationDtoList(provider.UkPrn, location.Postcode, isAdditionalData));
                }
            }

            var (retryPolicy, context) = _policyRegistry.GetRetryPolicy(_logger);

            await retryPolicy
                .ExecuteAsync(async _ =>
                        await PerformSave(providers, locationData, locationQualificationData, isAdditionalData),
                    context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when saving providers");
            throw;
        }
    }

    private async Task PerformSave(
        IEnumerable<Models.Provider> providers,
        IEnumerable<LocationDto> locationData,
        IEnumerable<LocationQualificationDto> locationQualificationData,
        bool isAdditionalData = false)
    {
        using var connection = _dbContextWrapper.CreateConnection();
        connection.Open();

        using var transaction = _dbContextWrapper.BeginTransaction(connection);

        var providerUpdateResult = await _dbContextWrapper
            .QueryAsync<(string Change, int ChangeCount)>(
                connection,
                "UpdateProviders",
                new
                {
                    data = providers.AsTableValuedParameter("dbo.ProviderDataTableType"),
                    isAdditionalData
                },
                transaction,
                commandType: CommandType.StoredProcedure);

        _logger.LogChangeResults(providerUpdateResult, nameof(ProviderRepository), nameof(providers));

        var locationUpdateResult = await _dbContextWrapper
            .QueryAsync<(string Change, int ChangeCount)>(
                connection,
                "UpdateLocations",
                new
                {
                    data = locationData.AsTableValuedParameter("dbo.LocationDataTableType"),
                    isAdditionalData
                },
                transaction,
                commandType: CommandType.StoredProcedure);

        _logger.LogChangeResults(locationUpdateResult, nameof(ProviderRepository), "locations");

        var locationQualificationUpdateResult = await _dbContextWrapper
            .QueryAsync<(string Change, int ChangeCount)>(
                connection,
                "UpdateLocationQualifications",
                new
                {
                    data = locationQualificationData.AsTableValuedParameter(
                        "dbo.LocationQualificationDataTableType"),
                    isAdditionalData
                },
                transaction,
                commandType: CommandType.StoredProcedure);

        _logger.LogChangeResults(locationQualificationUpdateResult, nameof(ProviderRepository),
            "location qualifications", includeUpdated: false);

        transaction.Commit();
    }

    public async Task<(IEnumerable<ProviderSearchResult> SearchResults, int TotalResultsCount)> Search(
        GeoLocation fromGeoLocation,
        IList<int> routeIds,
        IList<int> qualificationIds,
        int page,
        int pageSize,
        bool includeAdditionalData)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        var providerSearchResults = new Dictionary<string, ProviderSearchResult>();
        
        var parameters = new DynamicParameters(
                new
                {
                    fromLatitude = fromGeoLocation.Latitude,
                    fromLongitude = fromGeoLocation.Longitude,
                    routeIds = routeIds?.AsTableValuedParameter("dbo.IdListTableType"),
                    qualificationIds = qualificationIds?.AsTableValuedParameter("dbo.IdListTableType"),
                    page,
                    pageSize,
                    includeAdditionalData
                });
        parameters.Add("totalLocationsCount", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

        await _dbContextWrapper
            .QueryAsync<ProviderSearchResult, DeliveryYear, Qualification, ProviderSearchResult>(
                connection,
                "SearchProviders",
                map: (p, ly, q) =>
                {
                    var key = $"{p.UkPrn}_{p.Postcode}";
                    if (!providerSearchResults.TryGetValue(key, out var searchResult))
                    {
                        providerSearchResults.Add(key, searchResult = p);
                        searchResult.JourneyToLink = fromGeoLocation.CreateJourneyLink(searchResult.Postcode);
                    }

                    var deliveryYear = searchResult
                        .DeliveryYears
                        .FirstOrDefault(y => y.Year == ly.Year);

                    if (deliveryYear == null)
                    {
                        deliveryYear = ly;

                        deliveryYear.IsAvailableNow = deliveryYear.Year.IsAvailableAtDate(_dateTimeService.Today);

                        searchResult.DeliveryYears.Add(deliveryYear);
                    }

                    if (deliveryYear.Qualifications.All(z => z.Id != q.Id))
                    {
                        deliveryYear.Qualifications.Add(q);
                    }

                    return searchResult;
                },
                parameters,
                splitOn: "UkPrn, Postcode, Year, Id",
                commandType: CommandType.StoredProcedure);

        var totalLocationsCount = parameters.Get<int>("totalLocationsCount");

        var searchResults = providerSearchResults
            .Values
            .OrderBy(s => s.Distance)
            .ThenBy(s => s.ProviderName)
            .ThenBy(s => s.LocationName)
            .ToList();

        return (searchResults, totalLocationsCount);
    }
}