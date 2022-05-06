using Intertech.Facade.DapperParameters;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Repositories;

public class ProviderRepositoryBuilder
{
    public ProviderRepository Build(
        IDbContextWrapper dbContextWrapper = null,
        IDapperParameters dbParameters = null,
        IDateTimeService dateTimeService = null,
        IReadOnlyPolicyRegistry<string> policyRegistry = null,
        ILogger<ProviderRepository> logger = null)
    {
        dbContextWrapper ??= Substitute.For<IDbContextWrapper>();
        dateTimeService ??= Substitute.For<IDateTimeService>();
        dbParameters ??= Substitute.For<IDapperParameters>();
        policyRegistry ??= Substitute.For<IReadOnlyPolicyRegistry<string>>();

        logger ??= Substitute.For<ILogger<ProviderRepository>>();
        
        return new ProviderRepository(
            dbContextWrapper,
            dbParameters,
            dateTimeService,
            policyRegistry, 
            logger);
    }
}