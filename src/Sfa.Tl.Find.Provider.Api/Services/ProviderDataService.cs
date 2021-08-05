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
        private readonly ILogger<ProviderDataService> _logger;
        private readonly IProviderRepository _providerRepository;
        private readonly IQualificationRepository _qualificationRepository;

        public ProviderDataService(
            IProviderRepository providerRepository,
            IQualificationRepository qualificationRepository,
            ILogger<ProviderDataService> logger)
        {
            _providerRepository = providerRepository ?? throw new ArgumentNullException(nameof(providerRepository));
            _qualificationRepository = qualificationRepository ?? throw new ArgumentNullException(nameof(qualificationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Qualification>> GetQualifications()
        {
            _logger.LogDebug("Getting qualifications");

            return await _qualificationRepository.GetAllQualifications();
        }
        
        public async Task<IEnumerable<Models.Provider>> FindProviders(
            string postcode,
            int? qualificationId = null,
            int page = 0,
            int pageSize = Constants.DefaultPageSize)
        {
            _logger.LogDebug($"Searching for postcode {postcode}");

            return await _providerRepository.GetAllProviders();
        }
    }
}
