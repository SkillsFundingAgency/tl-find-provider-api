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

        public ProviderDataService(
            ILogger<ProviderDataService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Qualification>> GetQualifications()
        {
            _logger.LogDebug("Getting qualifications");

            //TODO: Move to QualificationDataRepository 
            return new Qualification[] 
            {
                new() { Id = 45, Name = "Building Services Engineering for Construction" },
                new() { Id = 36, Name = "Design, Surveying and Planning for Construction" },
                new() { Id = 39, Name = "Digital Business Services" },
                new() { Id = 37, Name = "Digital Production, Design and Development" },
                new() { Id = 40, Name = "Digital Support Services" },
                new() { Id = 38, Name = "Education and Childcare" },
                new() { Id = 41, Name = "Health" },
                new() { Id = 42, Name = "Healthcare Science" },
                new() { Id = 44, Name = "Onsite Construction" },
                new() { Id = 43, Name = "Science" },
                new() { Id = 46, Name = "Finance" },
                new() { Id = 47, Name = "Accounting" },
                new() { Id = 48, Name = "Design and development for engineering and manufacturing" },
                new() { Id = 49, Name = "Maintenance, installation and repair for engineering and manufacturing" },
                new() { Id = 50, Name = "Engineering, manufacturing, processing and control" },
                new() { Id = 51, Name = "Management and administration" }
            };
        }
        
        public async Task<IEnumerable<Models.Provider>> FindProviders(
            string postCode,
            int? qualificationId = null,
            int page = 0,
            int pageSize = Constants.DefaultPageSize)
        {
            _logger.LogDebug($"Searching for postcode {postCode}");

            //TODO: Move to ProviderDataRepository 
            return new Models.Provider[] 
            {
                new()
                {
                    UkPrn = 10000001,
                    Name = "Test provider"
                }
            };
        }
    }
}
