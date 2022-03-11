using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Services;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders;

public class TownDataServiceBuilder
{
    public TownDataService Build(
        ITownRepository townRepository = null,
        ILogger<TownDataService> logger = null)
    {
        townRepository ??= Substitute.For<ITownRepository>();
        logger ??= Substitute.For<ILogger<TownDataService>>();

        return new TownDataService(
            townRepository,
            logger);
    }
}