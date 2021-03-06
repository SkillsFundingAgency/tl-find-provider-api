using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Jobs;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Jobs;

public class InitializationJobBuilder
{
    public InitializationJob Build(
        ICourseDirectoryService courseDirectoryService = null,
        IProviderDataService providerDataService = null,
        ITownDataService townDataService = null,
        ILogger<InitializationJob> logger = null)
    {
        courseDirectoryService ??= Substitute.For<ICourseDirectoryService>();
        providerDataService ??= Substitute.For<IProviderDataService>();
        townDataService ??= Substitute.For<ITownDataService>();
        logger ??= Substitute.For<ILogger<InitializationJob>>();

        return new InitializationJob(
            courseDirectoryService,
            providerDataService,
            townDataService,
            logger);
    }
}