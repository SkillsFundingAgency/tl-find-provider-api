using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;
using Sfa.Tl.Find.Provider.Api.Models.Exceptions;

namespace Sfa.Tl.Find.Provider.Api.Services;

public class ProviderDataService : IProviderDataService
{
    private readonly IDateTimeService _dateTimeService;
    private readonly ITownDataService _townDataService;
    private readonly IPostcodeLookupService _postcodeLookupService;
    private readonly IProviderRepository _providerRepository;
    private readonly IQualificationRepository _qualificationRepository;
    private readonly IRouteRepository _routeRepository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ProviderDataService> _logger;
    private readonly bool _mergeAdditionalProviderData;

    public ProviderDataService(
        IDateTimeService dateTimeService,
        IPostcodeLookupService postcodeLookupService,
        IProviderRepository providerRepository,
        IQualificationRepository qualificationRepository,
        IRouteRepository routeRepository,
        ITownDataService townDataService,
        IMemoryCache cache,
        IOptions<SearchSettings> searchOptions,
        ILogger<ProviderDataService> logger)
    {
        _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
        _postcodeLookupService = postcodeLookupService ?? throw new ArgumentNullException(nameof(postcodeLookupService));
        _providerRepository = providerRepository ?? throw new ArgumentNullException(nameof(providerRepository));
        _qualificationRepository = qualificationRepository ?? throw new ArgumentNullException(nameof(qualificationRepository));
        _routeRepository = routeRepository ?? throw new ArgumentNullException(nameof(routeRepository));
        _townDataService = townDataService ?? throw new ArgumentNullException(nameof(townDataService));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _mergeAdditionalProviderData = searchOptions?.Value?.MergeAdditionalProviderData
                                       ?? throw new ArgumentNullException(nameof(searchOptions));
    }

    public async Task<IEnumerable<Qualification>> GetQualifications()
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Getting qualifications");
        }

        const string key = CacheKeys.QualificationsKey;
        if (!_cache.TryGetValue(key, out IList<Qualification> qualifications))
        {
            qualifications = (await _qualificationRepository
                .GetAll())
                .ToList();
            _cache.Set(key, qualifications,
                CacheUtilities.DefaultMemoryCacheEntryOptions(_dateTimeService, _logger));
        }

        return qualifications;
    }

    public async Task<IEnumerable<Route>> GetRoutes()
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Getting routes");
        }

        const string key = CacheKeys.RoutesKey;
        if (!_cache.TryGetValue(key, out IList<Route> routes))
        {
            routes = (await _routeRepository
                .GetAll(_mergeAdditionalProviderData))
                .ToList();
            _cache.Set(key, routes,
                CacheUtilities.DefaultMemoryCacheEntryOptions(_dateTimeService, _logger));
        }

        return routes;
    }

    public async Task<ProviderSearchResponse> FindProviders(
        string searchTerms,
        IList<int> routeIds = null,
        IList<int> qualificationIds = null,
        int page = 0,
        int pageSize = Constants.DefaultPageSize)
    {
        try
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Searching for postcode or town '{searchTerm}'", searchTerms);
            }

            GeoLocation geoLocation = null;

            if (searchTerms.IsFullOrPartialPostcode())
            {
                geoLocation = await GetPostcode(searchTerms);
            }
            else
            {
                var towns = (await _townDataService.Search(searchTerms)).ToList();

                var town = towns.Count switch
                {
                    1 => towns.Single(),
                    > 1 => towns.FirstOrDefault(t => string.Compare(t.FormatTownName(), searchTerms, StringComparison.CurrentCultureIgnoreCase) == 0)
                                                     ?? towns.FirstOrDefault(t => string.Compare(t.Name, searchTerms, StringComparison.CurrentCultureIgnoreCase) == 0)
                                                     ?? towns.FirstOrDefault(t => t.FormatTownName().StartsWith(searchTerms, StringComparison.CurrentCultureIgnoreCase)),
                    _ => null
                };

                if (town != null)
                {
                    var latitude = Convert.ToDouble(town.Latitude);
                    var longitude = Convert.ToDouble(town.Longitude);

                    geoLocation = new GeoLocation
                    {
                        Location = town.FormatTownName(),
                        Latitude = latitude,
                        Longitude = longitude
                    };
                }

                if (geoLocation == null)
                {
                    throw new PostcodeNotFoundException(searchTerms);
                }
            }

            var searchResults = await _providerRepository
                .Search(geoLocation,
                        routeIds,
                        qualificationIds,
                        page,
                        pageSize,
                        _mergeAdditionalProviderData);

            return new ProviderSearchResponse
            {
                SearchTerm = geoLocation.Location,
                SearchResults = searchResults
            };
        }
        catch (PostcodeNotFoundException pex)
        {
            _logger.LogError(pex, "Postcode {Postcode} was not found. Returning an error result.",
                pex.Postcode);
            return new ProviderSearchResponse
            {
                Error = "The postcode was not found"
            };
        }
    }

    public async Task<ProviderSearchResponse> FindProviders(
        double latitude,
        double longitude,
        IList<int> routeIds = null,
        IList<int> qualificationIds = null,
        int page = 0,
        int pageSize = Constants.DefaultPageSize)
    {
        try
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Searching for postcode near ({latitude}, {longitude})", latitude, longitude);
            }

            var geoLocation = await GetNearestPostcode(latitude, longitude);

            var searchResults = await _providerRepository
                .Search(geoLocation,
                    routeIds,
                    qualificationIds,
                    page,
                    pageSize,
                    _mergeAdditionalProviderData);

            return new ProviderSearchResponse
            {
                SearchTerm = geoLocation.Location,
                SearchResults = searchResults
            };
        }
        catch (PostcodeNotFoundException pex)
        {
            _logger.LogError(pex, "Postcode {Postcode} was not found. Returning an error result.",
                pex.Postcode);
            return new ProviderSearchResponse
            {
                Error = "The postcode was not found"
            };
        }
    }

    public async Task<bool> HasQualifications()
    {
        return await _qualificationRepository.HasAny();
    }

    public async Task<bool> HasProviders()
    {
        return await _providerRepository.HasAny();
    }

    private async Task<GeoLocation> GetPostcode(string postcode)
    {
        var key = CacheKeys.PostcodeKey(postcode);

        if (!_cache.TryGetValue(key, out GeoLocation geoLocation))
        {
            geoLocation = postcode.Length <= 4
                ? await _postcodeLookupService.GetOutcode(postcode)
                : await _postcodeLookupService.GetPostcode(postcode);

            if (geoLocation is null)
            {
                throw new PostcodeNotFoundException(postcode);
            }

            _cache.Set(key, geoLocation,
                CacheUtilities.DefaultMemoryCacheEntryOptions(
                    _dateTimeService,
                    _logger));
        }

        return geoLocation;
    }

    private async Task<GeoLocation> GetNearestPostcode(double latitude, double longitude)
    {
        var key = CacheKeys.LatLongKey(latitude, longitude);

        if (!_cache.TryGetValue(key, out GeoLocation geoLocation))
        {
            geoLocation = await _postcodeLookupService.GetNearestPostcode(latitude, longitude);

            _cache.Set(key, geoLocation,
                CacheUtilities.DefaultMemoryCacheEntryOptions(
                    _dateTimeService,
                    _logger));
        }

        return geoLocation;
    }

    public async Task LoadAdditionalProviderData()
    {
        //if (!_mergeAdditionalProviderData) return;

        try
        {
            const string jsonFile = "Assets.ProviderData.json";
            var providers = JsonDocument
                    .Parse(jsonFile.ReadManifestResourceStreamAsString())
                    .RootElement
                    .GetProperty("providers")
                    .EnumerateArray()
                    .Select(p =>
                        new Models.Provider
                        {
                            UkPrn = p.GetProperty("ukPrn").GetInt64(),
                            Name = p.GetProperty("name").GetString(),
                            Postcode = p.SafeGetString("postcode"),
                            Town = p.SafeGetString("town"),
                            Email = p.SafeGetString("email"),
                            Telephone = p.SafeGetString("telephone"),
                            Website = p.SafeGetString("website"),
                            IsAdditionalData = true,
                            Locations = p.GetProperty("locations")
                                .EnumerateArray()
                                .Select(l =>
                                    new Location
                                    {
                                        Postcode = l.GetProperty("postcode").GetString(),
                                        Name = l.SafeGetString("name", defaultValue: p.SafeGetString("name")),
                                        Town = l.SafeGetString("town"),
                                        Latitude = l.SafeGetDouble("latitude"),
                                        Longitude = l.SafeGetDouble("longitude"),
                                        Email = l.SafeGetString("email"),
                                        Telephone = l.SafeGetString("telephone"),
                                        Website = l.SafeGetString("website"),
                                        IsAdditionalData = true,
                                        DeliveryYears = l.TryGetProperty("deliveryYears", out var deliveryYears)
                                            ? deliveryYears.EnumerateArray()
                                                .Select(d =>
                                                    new DeliveryYear
                                                    {
                                                        Year = d.GetProperty("year").GetInt16(),
                                                        Qualifications = d.GetProperty("qualifications")
                                                            .EnumerateArray()
                                                            .Select(q => new Qualification
                                                            {
                                                                Id = q.GetInt32()
                                                            })
                                                            .ToList()
                                                    })
                                                .ToList()
                                            : new List<DeliveryYear>()
                                    }).ToList()
                        })
                    .ToList();

            await _providerRepository.Save(providers, true);

            _logger.LogInformation("Saved {providerCount} providers from ", providers.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(LoadAdditionalProviderData)} failed.");
            throw;
        }
    }
}