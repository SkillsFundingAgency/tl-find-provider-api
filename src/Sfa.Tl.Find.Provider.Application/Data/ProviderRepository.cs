using System.Data;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.Data;

public class ProviderRepository : IProviderRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly IDynamicParametersWrapper _dynamicParametersWrapper;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<ProviderRepository> _logger;
    private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;

    public ProviderRepository(
        IDbContextWrapper dbContextWrapper,
        IDynamicParametersWrapper dynamicParametersWrapper,
        IDateTimeProvider dateTimeProvider,
        IReadOnlyPolicyRegistry<string> policyRegistry,
        ILogger<ProviderRepository> logger)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        _dynamicParametersWrapper = dynamicParametersWrapper ?? throw new ArgumentNullException(nameof(dynamicParametersWrapper));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _policyRegistry = policyRegistry ?? throw new ArgumentNullException(nameof(policyRegistry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<ProviderDetail>> GetAll()
    {
        using var connection = _dbContextWrapper.CreateConnection();

        var providerDetailResults = new Dictionary<string, ProviderDetail>();

        await _dbContextWrapper
            .QueryAsync<ProviderDetail, LocationDetail, DeliveryYearDetail, RouteDetail, QualificationDetail, ProviderDetail>(
                connection,
                "GetAllProviders",
                (p, l, dy, r, q) =>
                {
                    var key = $"{p.UkPrn}";
                    if (!providerDetailResults.TryGetValue(key, out var provider))
                    {
                        providerDetailResults.Add(key, provider = p);
                    }

                    var location = provider
                        .Locations
                        .FirstOrDefault(loc => loc.Postcode == l.Postcode);

                    if (location == null)
                    {
                        location = l;
                        provider.Locations.Add(location);
                    }

                    var deliveryYear = location
                        .DeliveryYears
                        .FirstOrDefault(y => y.Year == dy.Year);

                    if (deliveryYear == null)
                    {
                        deliveryYear = dy;
                        deliveryYear.IsAvailableNow = deliveryYear.Year.IsAvailableAtDate(_dateTimeProvider.Today);

                        location.DeliveryYears.Add(deliveryYear);
                    }

                    if (deliveryYear.Routes.All(z => z.RouteId != r.RouteId))
                    {
                        deliveryYear.Routes.Add(new RouteDetail { RouteId = r.RouteId, RouteName = r.RouteName });
                    }

                    var route = deliveryYear
                        .Routes
                        .FirstOrDefault(rt => rt.RouteId == r.RouteId);

                    if (route != null && route.Qualifications.All(z => z.Id != q.Id))
                    {
                        route.Qualifications.Add(new QualificationDetail { Id = q.Id, Name = q.Name });
                    }

                    return provider;
                },
            splitOn: "UkPrn, Postcode, Year, RouteId, QualificationId",
            commandType: CommandType.StoredProcedure);

        var results = providerDetailResults
            .Values
            .OrderBy(p => p.Name)
            .ToList();

        return results;
    }

    public async Task<IEnumerable<ProviderDetailFlat>> GetAllFlattened()
    {
        using var connection = _dbContextWrapper.CreateConnection();

        return await _dbContextWrapper
            .QueryAsync<ProviderDetailFlat>(
                connection,
                "GetAllProviderDetails",
                commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<LocationPostcode>> GetLocationPostcodes(
        long ukPrn,
        bool includeAdditionalData)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            ukPrn
        });

        return await _dbContextWrapper
            .QueryAsync<LocationPostcode>(
                connection,
                "GetProviderLocations",
                _dynamicParametersWrapper.DynamicParameters,
                commandType: CommandType.StoredProcedure);
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

            var (retryPolicy, context) = _policyRegistry.GetDapperRetryPolicy(_logger);

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

        _dynamicParametersWrapper.CreateParameters(new
        {
            data = providers.AsTableValuedParameter("dbo.ProviderDataTableType"),
            isAdditionalData
        });

        var providerUpdateResult = await _dbContextWrapper
            .QueryAsync<(string Change, int ChangeCount)>(
                connection,
                "UpdateProviders",
                _dynamicParametersWrapper.DynamicParameters,
                transaction,
                commandType: CommandType.StoredProcedure);

        _logger.LogChangeResults(providerUpdateResult, nameof(ProviderRepository), nameof(providers));

        _dynamicParametersWrapper.CreateParameters(new
        {
            data = locationData.AsTableValuedParameter("dbo.LocationDataTableType"),
            isAdditionalData
        });

        var locationUpdateResult = await _dbContextWrapper
            .QueryAsync<(string Change, int ChangeCount)>(
                connection,
                "UpdateLocations",
                _dynamicParametersWrapper.DynamicParameters,
                transaction,
                commandType: CommandType.StoredProcedure);

        _logger.LogChangeResults(locationUpdateResult, nameof(ProviderRepository), "locations");

        _dynamicParametersWrapper.CreateParameters(new
        {
            data = locationQualificationData.AsTableValuedParameter("dbo.LocationQualificationDataTableType"),
            isAdditionalData
        });

        var locationQualificationUpdateResult = await _dbContextWrapper
            .QueryAsync<(string Change, int ChangeCount)>(
                connection,
                "UpdateLocationQualifications",
                _dynamicParametersWrapper.DynamicParameters,
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

        _dynamicParametersWrapper.CreateParameters(new
        {
            fromLatitude = fromGeoLocation.Latitude,
            fromLongitude = fromGeoLocation.Longitude,
            routeIds = routeIds?.AsTableValuedParameter("dbo.IdListTableType"),
            qualificationIds = qualificationIds?.AsTableValuedParameter("dbo.IdListTableType"),
            page,
            pageSize
        })
            .AddOutputParameter("totalLocationsCount", DbType.Int32);

        await _dbContextWrapper
            .QueryAsync<ProviderSearchResult, DeliveryYearSearchResult, RouteDto, QualificationDto, ProviderSearchResult>(
                connection,
            "SearchProviders",
            (p, ly, r, q) =>
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

                    deliveryYear.IsAvailableNow = deliveryYear.Year.IsAvailableAtDate(_dateTimeProvider.Today);

                    searchResult.DeliveryYears.Add(deliveryYear);
                }

                if (deliveryYear.Routes.All(z => z.Id != r.RouteId))
                {
                    deliveryYear.Routes.Add(new Route { Id = r.RouteId, Name = r.RouteName });
                }

                var route = deliveryYear
                    .Routes
                    .FirstOrDefault(rt => rt.Id == r.RouteId);

                if (route != null && route.Qualifications.All(z => z.Id != q.QualificationId))
                {
                    route.Qualifications.Add(new Qualification { Id = q.QualificationId, Name = q.QualificationName });
                }

                return searchResult;
            },
            _dynamicParametersWrapper.DynamicParameters,
            splitOn: "UkPrn, Postcode, Year, RouteId, QualificationId",
            commandType: CommandType.StoredProcedure);

        var totalLocationsCount = _dynamicParametersWrapper
            .DynamicParameters
            .Get<int>("totalLocationsCount");

        var searchResults = providerSearchResults
            .Values
            .OrderBy(s => s.Distance)
            .ThenBy(s => s.ProviderName)
            .ThenBy(s => s.LocationName)
            .ToList();

        return (searchResults, totalLocationsCount);
    }

    public async Task<int> UpdateProviderContacts(IEnumerable<ProviderContactDto> contacts)
    {
        using var connection = _dbContextWrapper.CreateConnection();
        connection.Open();

        var resultCount = 0;
        foreach (var contact in contacts)
        {
            _dynamicParametersWrapper.CreateParameters(new
            {
                contact.UkPrn,
                contact.EmployerContactEmail,
                contact.EmployerContactTelephone,
                contact.EmployerContactWebsite,
                contact.StudentContactEmail,
                contact.StudentContactTelephone,
                contact.StudentContactWebsite,
                currentTime = _dateTimeProvider.UtcNow
            });

            resultCount += await _dbContextWrapper.ExecuteAsync(
                connection,
                "UPDATE dbo.Provider " +
                "SET EmployerContactEmail = @EmployerContactEmail, " +
                "    EmployerContactTelephone = @EmployerContactTelephone, " +
                "    EmployerContactWebsite = @EmployerContactWebsite, " +
                "    StudentContactEmail = @StudentContactEmail, " +
                "    StudentContactTelephone = @StudentContactTelephone, " +
                "    StudentContactWebsite = @StudentContactWebsite, " +
                "    ModifiedOn = @currentTime " +
                "WHERE UkPrn = @UkPrn",
                _dynamicParametersWrapper.DynamicParameters);
        }

        return resultCount;
    }
}