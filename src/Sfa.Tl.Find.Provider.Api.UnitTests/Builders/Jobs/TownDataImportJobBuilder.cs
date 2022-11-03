using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Api.Jobs;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Jobs;

public class TownDataImportJobBuilder
{
    public TownDataImportJob Build(
        ITownDataService townDataService = null,
        ILogger<TownDataImportJob> logger = null)
    {
        townDataService ??= Substitute.For<ITownDataService>();
        logger ??= Substitute.For<ILogger<TownDataImportJob>>();

        return new TownDataImportJob(
            townDataService,
            logger);
    }
}