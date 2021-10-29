using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Jobs
{
    public class InitializationJob : IJob
    {
        private readonly ICourseDirectoryService _courseDirectoryService;
        private readonly IProviderDataService _providerDataService;
        private readonly ILogger<InitializationJob> _logger;

        public InitializationJob(
            ICourseDirectoryService courseDirectoryService,
            IProviderDataService providerDataService,
            ILogger<InitializationJob> logger)
        {
            _courseDirectoryService = courseDirectoryService ?? throw new ArgumentNullException(nameof(courseDirectoryService));
            _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"{nameof(InitializationJob)} job triggered. {context?.Trigger.JobKey.Name}");

            try
            {
                //Only populate data if there isn't any in the db yet
                if (!await _providerDataService.HasQualifications() ||
                    !await _providerDataService.HasProviders())
                {
                    await _courseDirectoryService.ImportQualifications();
                    await _courseDirectoryService.ImportProviders();
                }
                else
                {
                    _logger.LogInformation($"{nameof(InitializationJob)} did not call import because data already exists.");
                }

                _logger.LogInformation($"{nameof(InitializationJob)} job completed successfully.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{nameof(InitializationJob)} job failed.");
            }
        }
    }
}
