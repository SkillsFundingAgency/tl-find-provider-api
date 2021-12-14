using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.Models.Exceptions;

namespace Sfa.Tl.Find.Provider.Api.Services
{
    public class ProviderDataService : IProviderDataService
    {
        private readonly IDateTimeService _dateTimeService;
        private readonly IPostcodeLookupService _postcodeLookupService;
        private readonly IProviderRepository _providerRepository;
        private readonly IQualificationRepository _qualificationRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProviderDataService> _logger;

        public ProviderDataService(
            IDateTimeService dateTimeService,
            IPostcodeLookupService postcodeLookupService,
            IProviderRepository providerRepository,
            IQualificationRepository qualificationRepository,
            IRouteRepository routeRepository,
            IMemoryCache cache,
            ILogger<ProviderDataService> logger)
        {
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            _postcodeLookupService = postcodeLookupService ?? throw new ArgumentNullException(nameof(postcodeLookupService));
            _providerRepository = providerRepository ?? throw new ArgumentNullException(nameof(providerRepository));
            _qualificationRepository = qualificationRepository ?? throw new ArgumentNullException(nameof(qualificationRepository));
            _routeRepository = routeRepository ?? throw new ArgumentNullException(nameof(routeRepository));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Qualification>> GetQualifications()
        {
            _logger.LogDebug("Getting qualifications");

            const string key = CacheKeys.QualificationsKey;
            if (!_cache.TryGetValue(key, out IList<Qualification> qualifications))
            {
                qualifications = (await _qualificationRepository.GetAll()).ToList();
                _cache.Set(key, qualifications,
                    CacheUtilities.DefaultMemoryCacheEntryOptions(_dateTimeService, _logger));
            }

            return qualifications;
        }

        public async Task<IEnumerable<Route>> GetRoutes()
        {
            _logger.LogDebug("Getting routes");

            const string key = CacheKeys.RoutesKey;
            if (!_cache.TryGetValue(key, out IList<Route> routes))
            {
                routes = (await _routeRepository.GetAll()).ToList();
                _cache.Set(key, routes,
                    CacheUtilities.DefaultMemoryCacheEntryOptions(_dateTimeService, _logger));
            }

            return routes;
        }

        public async Task<ProviderSearchResponse> FindProviders(
            string postcode,
            int? qualificationId = null,
            int page = 0,
            int pageSize = Constants.DefaultPageSize)
        {
            try
            {
                _logger.LogDebug("Searching for postcode {postcode}", postcode);

                var postcodeLocation = await GetPostcode(postcode);

                var searchResults = await _providerRepository.Search(postcodeLocation, qualificationId, page, pageSize);
                return new ProviderSearchResponse
                {
                    Postcode = postcodeLocation.Postcode,
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

        private async Task<PostcodeLocation> GetPostcode(string postcode)
        {
            var key = CacheKeys.PostcodeKey(postcode);

            if (!_cache.TryGetValue(key, out PostcodeLocation postcodeLocation))
            {
                postcodeLocation = postcode.Length <= 4
                    ? await _postcodeLookupService.GetOutcode(postcode)
                    : await _postcodeLookupService.GetPostcode(postcode);

                if (postcodeLocation is null)
                {
                    throw new PostcodeNotFoundException(postcode);
                }

                _cache.Set(key, postcodeLocation,
                    CacheUtilities.DefaultMemoryCacheEntryOptions(
                        _dateTimeService,
                        _logger));
            }

            return postcodeLocation;
        }
    }
}
