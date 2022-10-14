using Microsoft.Extensions.Logging;
using NSubstitute;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;

public class EmployerInterestRepositoryBuilder
{
    public IEmployerInterestRepository Build(
        IDbContextWrapper dbContextWrapper = null,
        IDynamicParametersWrapper dynamicParametersWrapper = null,
        IReadOnlyPolicyRegistry<string> policyRegistry = null,
        ILogger<EmployerInterestRepository> logger = null)
    {
        dbContextWrapper ??= Substitute.For<IDbContextWrapper>();
        dynamicParametersWrapper ??= Substitute.For<IDynamicParametersWrapper>();
        policyRegistry ??= Substitute.For<IReadOnlyPolicyRegistry<string>>();
        logger ??= Substitute.For<ILogger<EmployerInterestRepository>>();

        return new EmployerInterestRepository(
            dbContextWrapper,
            dynamicParametersWrapper,
            policyRegistry,
            logger);
    }
}