using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Services
{
    public class TownDataService : ITownDataService
    {
        private readonly ITownRepository _townRepository;
        private readonly ILogger<TownDataService> _logger;

        public TownDataService(
            ITownRepository townRepository,
            ILogger<TownDataService> logger)
        {
            _townRepository = townRepository ?? throw new ArgumentNullException(nameof(townRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> HasTowns()
        {
            return await _townRepository.HasAny();
        }

        public async Task ImportTowns()
        {
            
        }
    }
}
