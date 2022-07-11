using Microsoft.Extensions.Logging;
using NSubstitute;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Repositories;

public class ProviderRepositoryBuilder
{
    public ProviderRepository Build(
        IDbContextWrapper dbContextWrapper = null,
        IDynamicParametersWrapper dynamicParametersWrapper = null,
        IDateTimeService dateTimeService = null,
        IReadOnlyPolicyRegistry<string> policyRegistry = null,
        ILogger<ProviderRepository> logger = null)
    {
        dbContextWrapper ??= Substitute.For<IDbContextWrapper>();
        dateTimeService ??= Substitute.For<IDateTimeService>();
        dynamicParametersWrapper ??= Substitute.For<IDynamicParametersWrapper>();
        policyRegistry ??= Substitute.For<IReadOnlyPolicyRegistry<string>>();

        logger ??= Substitute.For<ILogger<ProviderRepository>>();
        
        return new ProviderRepository(
            dbContextWrapper,
            dynamicParametersWrapper,
            dateTimeService,
            policyRegistry, 
            logger);
    }
}