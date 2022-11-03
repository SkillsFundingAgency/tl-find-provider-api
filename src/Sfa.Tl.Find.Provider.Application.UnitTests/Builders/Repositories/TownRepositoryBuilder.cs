using Microsoft.Extensions.Logging;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;

public class TownRepositoryBuilder
{
    public ITownRepository Build(
        IDbContextWrapper dbContextWrapper = null,
        IDynamicParametersWrapper dynamicParametersWrapper = null,
        IReadOnlyPolicyRegistry<string> policyRegistry = null,
        ILogger<TownRepository> logger = null)
    {
        dbContextWrapper ??= Substitute.For<IDbContextWrapper>();
        dynamicParametersWrapper ??= Substitute.For<IDynamicParametersWrapper>();
        policyRegistry ??= Substitute.For<IReadOnlyPolicyRegistry<string>>();
        logger ??= Substitute.For<ILogger<TownRepository>>();

        return new TownRepository(
            dbContextWrapper,
            dynamicParametersWrapper,
            policyRegistry,
            logger);
    }
}