using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProviderDataService> _logger;

        public ProviderDataService(
            IDateTimeService dateTimeService,
            IPostcodeLookupService postcodeLookupService,
            IProviderRepository providerRepository,
            IQualificationRepository qualificationRepository,
            IMemoryCache cache,
            ILogger<ProviderDataService> logger)
        {
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            _postcodeLookupService = postcodeLookupService ?? throw new ArgumentNullException(nameof(postcodeLookupService));
            _providerRepository = providerRepository ?? throw new ArgumentNullException(nameof(providerRepository));
            _qualificationRepository = qualificationRepository ?? throw new ArgumentNullException(nameof(qualificationRepository));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Qualification>> GetQualifications()
        {
            _logger.LogDebug("Getting qualifications");
            
            return await _qualificationRepository.GetAll();
        }
        
        public async Task<IEnumerable<ProviderSearchResult>> FindProviders(
            string postcode,
            int? qualificationId = null,
            int page = 0,
            int pageSize = Constants.DefaultPageSize)
        {
            _logger.LogDebug($"Searching for postcode {postcode}");

            var postcodeLocation = await GetPostcode(postcode);

            return await _providerRepository.Search(postcodeLocation, qualificationId, page, pageSize);
        }

        private async Task<PostcodeLocation> GetPostcode(string postcode)
        {
            var key = $"POSTCODE__{postcode.Replace(" ", "").ToUpper()}";
            if(_cache.TryGetValue(key, out PostcodeLocation postcodeLocation))
            {
                return postcodeLocation;
            }

            postcodeLocation = await _postcodeLookupService.GetPostcode(postcode);

            if (postcodeLocation is null)
            {
                throw new PostcodeNotFoundException(postcode);
            }

            var cacheExpiryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = _dateTimeService.Now.AddMinutes(60),
                Priority = CacheItemPriority.Normal,
                SlidingExpiration = TimeSpan.FromMinutes(10),
                Size = 1,
                PostEvictionCallbacks =
                {
                    new PostEvictionCallbackRegistration
                    {
                        EvictionCallback = Extensions.CacheExtensions.EvictionLoggingCallback,
                        State = _logger
                    }
                }
            };

            _cache.Set(key, postcodeLocation, cacheExpiryOptions);
            
            return postcodeLocation;
        }
    }
}
