using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Jobs;

public class ProviderReferenceImportJob : IJob
{
    private readonly IProviderReferenceDataService _providerReferenceDataService;
    private readonly ILogger<ProviderReferenceImportJob> _logger;

    public ProviderReferenceImportJob(
        IProviderReferenceDataService providerReferenceDataService,
        ILogger<ProviderReferenceImportJob> logger)
    {
        _providerReferenceDataService = providerReferenceDataService ?? throw new ArgumentNullException(nameof(providerReferenceDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{nameof(ProviderReferenceImportJob)} job triggered. {context?.Trigger.JobKey.Name}");

        try
        {
            //Only populate data if there isn't any in the db yet
            if (!await _providerReferenceDataService.HasProviderReferences())
            {
                //TODO: If we use this, it should be done after course directory import
                var data = await _providerReferenceDataService.GetAllSinceLastUpdate();

                await _providerReferenceDataService.Save(data);
            }

            _logger.LogInformation($"{nameof(ProviderReferenceImportJob)} job completed successfully.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{nameof(ProviderReferenceImportJob)} job failed.");
        }
    }
}