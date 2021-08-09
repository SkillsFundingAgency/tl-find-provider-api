using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Services
{
    public class ProviderDataService : IProviderDataService
    {
        private readonly ICourseDirectoryService _courseDirectoryService;
        private readonly IPostcodeLookupService _postcodeLookupService;
        private readonly IProviderRepository _providerRepository;
        private readonly IQualificationRepository _qualificationRepository;
        private readonly ILogger<ProviderDataService> _logger;

        public ProviderDataService(
            ICourseDirectoryService courseDirectoryService,
            IPostcodeLookupService postcodeLookupService,
            IProviderRepository providerRepository,
            IQualificationRepository qualificationRepository,
            ILogger<ProviderDataService> logger)
        {
            _courseDirectoryService = courseDirectoryService ?? throw new ArgumentNullException(nameof(courseDirectoryService));
            _postcodeLookupService = postcodeLookupService ?? throw new ArgumentNullException(nameof(postcodeLookupService));
            _providerRepository = providerRepository ?? throw new ArgumentNullException(nameof(providerRepository));
            _qualificationRepository = qualificationRepository ?? throw new ArgumentNullException(nameof(qualificationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Qualification>> GetQualifications()
        {
            _logger.LogDebug("Getting qualifications");

            //Temp for testing - load qualifications from API first
            await _courseDirectoryService.ImportQualifications();

            return await _qualificationRepository.GetAllQualifications();
        }
        
        public async Task<IEnumerable<Models.Provider>> FindProviders(
            string postcode,
            int? qualificationId = null,
            int page = 0,
            int pageSize = Constants.DefaultPageSize)
        {
            _logger.LogDebug($"Searching for postcode {postcode}");

            var postcodeLocation = await _postcodeLookupService.GetPostcode(postcode);
            //TODO: Check the postcode was valid and perform search

            return await _providerRepository.GetAllProviders();
        }
    }
}
