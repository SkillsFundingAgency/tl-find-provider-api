using CsvHelper;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.ClassMaps;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Exceptions;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using System.Globalization;

namespace Sfa.Tl.Find.Provider.Application.Services;

public class ProviderDataService : IProviderDataService
{
    private readonly IPostcodeLookupService _postcodeLookupService;
    private readonly IEmailService _emailService;
    private readonly ITownDataService _townDataService;
    private readonly IProviderRepository _providerRepository;
    private readonly IQualificationRepository _qualificationRepository;
    private readonly IRouteRepository _routeRepository;
    private readonly IIndustryRepository _industryRepository;
    private readonly ICacheService _cacheService;
    private readonly ProviderSettings _providerSettings;
    private readonly ILogger<ProviderDataService> _logger;

    public ProviderDataService(
        IPostcodeLookupService postcodeLookupService,
        IEmailService emailService,
        IProviderRepository providerRepository,
        IQualificationRepository qualificationRepository,
        IRouteRepository routeRepository,
        IIndustryRepository industryRepository,
        ITownDataService townDataService,
        ICacheService cacheService,
        IOptions<ProviderSettings> providerOptions,
        ILogger<ProviderDataService> logger)
    {
        _postcodeLookupService = postcodeLookupService ?? throw new ArgumentNullException(nameof(postcodeLookupService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _providerRepository = providerRepository ?? throw new ArgumentNullException(nameof(providerRepository));
        _qualificationRepository = qualificationRepository ?? throw new ArgumentNullException(nameof(qualificationRepository));
        _routeRepository = routeRepository ?? throw new ArgumentNullException(nameof(routeRepository));
        _industryRepository = industryRepository ?? throw new ArgumentNullException(nameof(industryRepository));
        _townDataService = townDataService ?? throw new ArgumentNullException(nameof(townDataService));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (providerOptions is null) throw new ArgumentNullException(nameof(providerOptions));

        _providerSettings = providerOptions?.Value
                            ?? throw new ArgumentNullException(nameof(providerOptions));
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

        const int HairdressingBarberingAndBeautyTherapyId = 53;
        return qualifications.Where(q => q.Id != HairdressingBarberingAndBeautyTherapyId).ToList();
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
            const int HairAndBeautyRouteId = 9;
            const int CateringIdRouteId = 3;
            const int SalesMarketingProcurementRouteId = 12;

            var excludedRoutes = new int[] { HairAndBeautyRouteId, CateringIdRouteId, SalesMarketingProcurementRouteId };

            routes = (await _routeRepository
                .GetAll())
                .Where(r => !excludedRoutes.Contains(r.Id))
                .ToList();
            await _cacheService.Set(key, routes);
        }

        return routes;
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
            .GetLocationPostcodes(ukPrn);
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
                    pageSize);

        return new ProviderSearchResponse
        {
            SearchTerm = geoLocation.Location,
            SearchResults = searchResults.ToList(),
            TotalResults = totalSearchResults
        };
    }

    private async Task SendProviderVerificationEmail(string emailAddress, Guid token)
    {
        var siteUri = new Uri(_providerSettings.ConnectSiteUri);
        var notificationsUri = new Uri(siteUri, "notifications");
        var verificationUri = new Uri(QueryHelpers.AddQueryString(
            notificationsUri.AbsoluteUri,
            "token",
            token.ToString("D").ToLower()));

        await _emailService.SendEmail(
            emailAddress,
            EmailTemplateNames.ProviderVerification,
            new Dictionary<string, string>
            {
                { "email_verification_link", verificationUri.ToString() },
                { "notifications_uri", notificationsUri.ToString() }
            },
            token.ToString());
    }
}