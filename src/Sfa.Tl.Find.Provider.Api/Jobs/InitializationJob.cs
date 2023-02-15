using Quartz;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Jobs;

public class InitializationJob : IJob
{
    private readonly ICourseDirectoryService _courseDirectoryService;
    private readonly ITownDataService _townDataService;
    private readonly IProviderDataService _providerDataService;
    private readonly ILogger<InitializationJob> _logger;

    public InitializationJob(
        ICourseDirectoryService courseDirectoryService,
        IProviderDataService providerDataService,
        ITownDataService townDataService,
        ILogger<InitializationJob> logger)
    {
        _courseDirectoryService = courseDirectoryService ?? throw new ArgumentNullException(nameof(courseDirectoryService));
        _providerDataService = providerDataService ?? throw new ArgumentNullException(nameof(providerDataService));
        _townDataService = townDataService ?? throw new ArgumentNullException(nameof(townDataService));
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
                _logger.LogInformation($"{nameof(InitializationJob)} importing providers and qualifications because no data exists.");
                //await _courseDirectoryService.ImportQualifications();
                await _courseDirectoryService.ImportProviders();
            }

            if (!await _townDataService.HasTowns())
            {
                _logger.LogInformation($"{nameof(InitializationJob)} importing towns because no data exists.");
                await _townDataService.ImportTowns();
            }
            
            _logger.LogInformation($"{nameof(InitializationJob)} job completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(InitializationJob)} job failed.");
        }
    }
}