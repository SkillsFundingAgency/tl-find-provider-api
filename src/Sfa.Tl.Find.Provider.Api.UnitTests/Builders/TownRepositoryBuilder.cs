using Microsoft.Extensions.Logging;
using NSubstitute;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders;

public class TownRepositoryBuilder
{
    public TownRepository Build(
        IDbContextWrapper dbContextWrapper = null,
        IReadOnlyPolicyRegistry<string> policyRegistry = null,
        ILogger<TownRepository> logger = null)
    {
        dbContextWrapper ??= Substitute.For<IDbContextWrapper>();
        policyRegistry ??= Substitute.For<IReadOnlyPolicyRegistry<string>>();
        logger ??= Substitute.For<ILogger<TownRepository>>();

        return new TownRepository(
            dbContextWrapper,
            policyRegistry,
            logger);
    }
}