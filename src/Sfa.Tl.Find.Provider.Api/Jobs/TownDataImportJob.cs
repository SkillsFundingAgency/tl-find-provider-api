﻿using Quartz;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Jobs;

public class TownDataImportJob : IJob
{
    private readonly ITownDataService _townDataService;
    private readonly ILogger<TownDataImportJob> _logger;

    public TownDataImportJob(
        ITownDataService townDataService,
        ILogger<TownDataImportJob> logger)
    {
        _townDataService = townDataService ?? throw new ArgumentNullException(nameof(townDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{nameof(TownDataImportJob)} job triggered. {context?.Trigger.JobKey.Name}");

        try
        {
            await _townDataService.ImportTowns();

            _logger.LogInformation($"{nameof(TownDataImportJob)} job completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(TownDataImportJob)} job failed.");
        }
    }
}