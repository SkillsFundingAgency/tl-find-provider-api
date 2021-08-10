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
        private readonly ICourseDirectoryService _courseDirectoryService;
        private readonly IPostcodeLookupService _postcodeLookupService;
        private readonly IProviderRepository _providerRepository;
        private readonly IQualificationRepository _qualificationRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProviderDataService> _logger;

        public ProviderDataService(
            ICourseDirectoryService courseDirectoryService,
            IPostcodeLookupService postcodeLookupService,
            IProviderRepository providerRepository,
            IQualificationRepository qualificationRepository,
            IMemoryCache cache,
            ILogger<ProviderDataService> logger)
        {
            _courseDirectoryService = courseDirectoryService ?? throw new ArgumentNullException(nameof(courseDirectoryService));
            _postcodeLookupService = postcodeLookupService ?? throw new ArgumentNullException(nameof(postcodeLookupService));
            _providerRepository = providerRepository ?? throw new ArgumentNullException(nameof(providerRepository));
            _qualificationRepository = qualificationRepository ?? throw new ArgumentNullException(nameof(qualificationRepository));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Qualification>> GetQualifications()
        {
            _logger.LogDebug("Getting qualifications");

            //Temp for testing - load qualifications from API first
            await _courseDirectoryService.ImportQualifications();

            return await _qualificationRepository.GetAll();
        }
        
        public async Task<IEnumerable<Models.Provider>> FindProviders(
            string postcode,
            int? qualificationId = null,
            int page = 0,
            int pageSize = Constants.DefaultPageSize)
        {
            _logger.LogDebug($"Searching for postcode {postcode}");

            var postcodeLocation = await GetPostcode(postcode);

            return await _providerRepository.GetAll();
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
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                Priority = CacheItemPriority.Normal,
                SlidingExpiration = TimeSpan.FromMinutes(10),
                Size = 1,
                PostEvictionCallbacks =
                {
                    new PostEvictionCallbackRegistration
                    {
                        EvictionCallback = EvictionCallback,
                        State = _logger
                    }
                }
            };

            _cache.Set(key, postcodeLocation, cacheExpiryOptions);
            
            return postcodeLocation;
        }

        private static void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            //TODO: Could move this to a class and call it EvictionCallbackLogger
            //var logger = (state as ProviderDataService)?._logger;
            var logger = (state as ILogger);
            logger?.LogInformation($"Entry {key} was evicted from the cache. Reason: {reason}.");
        }
    }
}
