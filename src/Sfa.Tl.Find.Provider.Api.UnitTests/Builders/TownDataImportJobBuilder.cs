using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Jobs;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders;

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