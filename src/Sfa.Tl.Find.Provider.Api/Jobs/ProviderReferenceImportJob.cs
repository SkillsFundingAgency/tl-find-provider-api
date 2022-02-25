using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Jobs;

public class ProviderReferenceImportJob : IJob
{
    private readonly IDateTimeService _dateTimeService;
    private readonly IProviderReferenceDataService _providerReferenceDataService;
    private readonly IProviderReferenceRepository _providerReferenceRepository;
    private readonly ILogger<ProviderReferenceImportJob> _logger;

    public ProviderReferenceImportJob(
        IProviderReferenceDataService providerReferenceDataService,
        IProviderReferenceRepository providerReferenceRepository,
        IDateTimeService dateTimeService,
        ILogger<ProviderReferenceImportJob> logger)
    {
        _providerReferenceDataService = providerReferenceDataService ?? throw new ArgumentNullException(nameof(providerReferenceDataService));
        _providerReferenceRepository = providerReferenceRepository ?? throw new ArgumentNullException((nameof(providerReferenceRepository)));
        _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{nameof(ProviderReferenceImportJob)} job triggered. {context?.Trigger.JobKey.Name}");

        try
        {
            //Only populate data if there isn't any in the db yet
            if (!await _providerReferenceRepository.HasAny())
            {
                //TODO: Read repository to get latest import date
                //TODO: If we use this, don't pull in the repository here - move into the service
                //TODO: If we use this, it should be done after course directory import
                var lastImportDate = _dateTimeService.MinValue;
                var data = await _providerReferenceDataService.GetAllAsync(lastImportDate);

                await _providerReferenceRepository.Save(data);
            }

            _logger.LogInformation($"{nameof(ProviderReferenceImportJob)} job completed successfully.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{nameof(ProviderReferenceImportJob)} job failed.");
        }
    }
}