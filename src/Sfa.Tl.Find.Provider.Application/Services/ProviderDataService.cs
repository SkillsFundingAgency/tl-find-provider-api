using System.Globalization;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.ClassMaps;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Application.Models.Exceptions;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.Services;

public class ProviderDataService : IProviderDataService
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPostcodeLookupService _postcodeLookupService;
    private readonly IEmailService _emailService;
    private readonly ITownDataService _townDataService;
    private readonly IProviderRepository _providerRepository;
    private readonly IQualificationRepository _qualificationRepository;
    private readonly IRouteRepository _routeRepository;
    private readonly IIndustryRepository _industryRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly ISearchFilterRepository _searchFilterRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ProviderDataService> _logger;
    private readonly bool _mergeAdditionalProviderData;

    public ProviderDataService(
        IDateTimeProvider dateTimeProvider,
        IPostcodeLookupService postcodeLookupService,
        IEmailService emailService,
        IProviderRepository providerRepository,
        IQualificationRepository qualificationRepository,
        IRouteRepository routeRepository,
        IIndustryRepository industryRepository,
        INotificationRepository notificationRepository,
        ISearchFilterRepository searchFilterRepository,
        ITownDataService townDataService,
        ICacheService cacheService,
        IOptions<SearchSettings> searchOptions,
        ILogger<ProviderDataService> logger)
    {
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _postcodeLookupService = postcodeLookupService ?? throw new ArgumentNullException(nameof(postcodeLookupService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _providerRepository = providerRepository ?? throw new ArgumentNullException(nameof(providerRepository));
        _qualificationRepository = qualificationRepository ?? throw new ArgumentNullException(nameof(qualificationRepository));
        _routeRepository = routeRepository ?? throw new ArgumentNullException(nameof(routeRepository));
        _industryRepository = industryRepository ?? throw new ArgumentNullException(nameof(industryRepository));
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _searchFilterRepository = searchFilterRepository ?? throw new ArgumentNullException(nameof(searchFilterRepository));
        _townDataService = townDataService ?? throw new ArgumentNullException(nameof(townDataService));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _mergeAdditionalProviderData = searchOptions?.Value?.MergeAdditionalProviderData
                                       ?? throw new ArgumentNullException(nameof(searchOptions));
    }

    public async Task<IEnumerable<Industry>> GetIndustries()
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Getting industries");
        }

        const string key = CacheKeys.IndustriesKey;
        var industries = await _cacheService.Get<IList<Industry>?>(key);
        if (industries is null)
        {
            industries = (await _industryRepository
                    .GetAll())
                .ToList();
            await _cacheService.Set(key, industries);
        }

        return industries;
    }

    public async Task<IEnumerable<Qualification>> GetQualifications()
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Getting qualifications");
        }

        const string key = CacheKeys.QualificationsKey;
        var qualifications = await _cacheService.Get<IList<Qualification>?>(key);
        if (qualifications is null)
        {
            qualifications = (await _qualificationRepository
                .GetAll())
                .ToList();
            await _cacheService.Set(key, qualifications);
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
        var routes = await _cacheService.Get<IList<Route>?>(key);
        if (routes is null)
        {
            routes = (await _routeRepository
                .GetAll(_mergeAdditionalProviderData))
                .ToList();
            await _cacheService.Set(key, routes);
        }

        return routes;
    }

    public async Task DeleteNotification(int notificationId)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Deleting notification {notificationId}", notificationId);
        }

        await _notificationRepository.Delete(notificationId);
    }

    public async Task<IEnumerable<NotificationSummary>> GetNotificationSummaryList(long ukPrn)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Getting notifications");
        }

        var notifications = 
            (await _notificationRepository
            .GetNotificationSummaryList(ukPrn, _mergeAdditionalProviderData));

        return notifications;
    }

    public async Task<Notification> GetNotification(int notificationId)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Getting notification {notificationId}", notificationId);
        }

        return await _notificationRepository
            .GetNotification(notificationId);
    }

    public async Task<IEnumerable<SearchFilter>> GetSearchFilterSummaryList(long ukPrn)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Getting search filters");
        }

        var searchFilters = (await _searchFilterRepository
            .GetSearchFilterSummaryList(ukPrn, _mergeAdditionalProviderData));

        return searchFilters;
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

    public async Task<ProviderSearchResponse> FindProviders(
        string searchTerms,
        IList<int> routeIds = null,
        IList<int> qualificationIds = null,
        int page = 0,
        int pageSize = Models.Constants.DefaultPageSize)
    {
        try
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Searching for postcode or town '{searchTerm}'", searchTerms);
            }

            GeoLocation geoLocation = null!;

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

            return await Search(routeIds, qualificationIds, page, pageSize, geoLocation);
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
        int pageSize = Models.Constants.DefaultPageSize)
    {
        try
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Searching for postcode near ({latitude}, {longitude})", latitude, longitude);
            }

            var geoLocation = await GetNearestPostcode(latitude, longitude);

            return await Search(routeIds, qualificationIds, page, pageSize, geoLocation);
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

    public async Task<ProviderDetailResponse> GetAllProviders()
    {
        return new ProviderDetailResponse
        {
            Providers = await _providerRepository
                .GetAll()
        };
    }

    public async Task<byte[]> GetCsv()
    {
        var providers = await _providerRepository
            .GetAllFlattened();

        await using var stream = new MemoryStream();
        await using var streamWriter = new StreamWriter(stream);
        await using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
        {
            csvWriter.Context.RegisterClassMap<ProviderDetailFlatClassMap>();
            await csvWriter.WriteRecordsAsync(providers);
        }

        return stream.ToArray();
    }

    public async Task<IEnumerable<LocationPostcode>> GetLocationPostcodes(long ukPrn)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Getting location postcodes for UKPRN {ukPrn}", ukPrn);
        }

        return await _providerRepository
            .GetLocationPostcodes(ukPrn, _mergeAdditionalProviderData);
    }

    public async Task ImportProviderContacts(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var csvReader = new CsvReader(reader,
            new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                PrepareHeaderForMatch = args =>
                {
                    if (string.Compare(args.Header, "UKPRN", StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        return "UkPrn";
                    }

                    return args.Header
                            .Replace(" ", "");
                },
                MissingFieldFound = _ => { /* ignore empty column values */ }
            });

        csvReader.Context.TypeConverterOptionsCache.GetOptions<string>()
            .NullValues
            .AddRange(new[] { "", "NULL", "NA", "N/A" });

        var contacts = csvReader
            .GetRecords<ProviderContactDto>()
            .ToList();

        var resultCount = await _providerRepository.UpdateProviderContacts(contacts);

        _logger.LogInformation("Updated contacts for {resultCount} providers from {contactsCount}.",
            resultCount, contacts.Count);
    }

    public async Task<bool> HasQualifications()
    {
        return await _qualificationRepository.HasAny();
    }

    public async Task<bool> HasProviders()
    {
        return await _providerRepository.HasAny();
    }

    public async Task ImportProviderData(Stream stream, bool isAdditionalData)
    {
        var jsonDocument = await JsonDocument
            .ParseAsync(stream);

        var count = await LoadAdditionalProviderData(jsonDocument);
        await ClearCaches();

        _logger.LogInformation("Loaded {count} providers from stream.", count);
    }

    public async Task SaveSearchFilter(SearchFilter searchFilter)
    {
        await _searchFilterRepository.Save(searchFilter);
    }

    public async Task SendEmailVerification(int notificationId)
    {
        //TODO: send email
    }

    private async Task<int> LoadAdditionalProviderData(JsonDocument jsonDocument)
    {
        try
        {
            var providers = jsonDocument
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

            return providers.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(LoadAdditionalProviderData)} failed.");
            throw;
        }
    }

    private async Task<GeoLocation> GetPostcode(string postcode)
    {
        var geoLocation = postcode.Length <= 4
                ? await _postcodeLookupService.GetOutcode(postcode)
                : await _postcodeLookupService.GetPostcode(postcode);

        if (geoLocation is null)
        {
            throw new PostcodeNotFoundException(postcode);
        }

        return geoLocation;
    }

    private async Task<GeoLocation> GetNearestPostcode(double latitude, double longitude)
    {
        var key = CacheKeys.LatLongKey(latitude, longitude);

        var geoLocation = await _cacheService.Get<GeoLocation?>(key);
        if (geoLocation is null)
        {
            geoLocation = await _postcodeLookupService.GetNearestPostcode(latitude, longitude);

            await _cacheService.Set(key, geoLocation, CacheDuration.Medium);
        }

        return geoLocation;
    }

    private async Task<ProviderSearchResponse> Search(IList<int> routeIds, IList<int> qualificationIds, int page, int pageSize, GeoLocation geoLocation)
    {
        var (searchResults, totalSearchResults) =
            await _providerRepository
                .Search(geoLocation,
                    routeIds,
                    qualificationIds,
                    page,
                    pageSize,
                    _mergeAdditionalProviderData);

        return new ProviderSearchResponse
        {
            SearchTerm = geoLocation.Location,
            SearchResults = searchResults.ToList(),
            TotalResults = totalSearchResults
        };
    }

    private async Task ClearCaches()
    {
        await _cacheService.Remove<IList<Qualification>>(CacheKeys.QualificationsKey);
        await _cacheService.Remove<IList<Route>>(CacheKeys.RoutesKey);
        await _cacheService.Remove<ProviderDataDownloadInfoResponse>(CacheKeys.ProviderDataDownloadInfoKey);
    }
}