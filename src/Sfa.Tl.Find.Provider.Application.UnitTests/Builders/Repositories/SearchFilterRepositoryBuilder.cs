using Microsoft.Extensions.Logging;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;

public class SearchFilterRepositoryBuilder
{
    public ISearchFilterRepository Build(
        IDbContextWrapper dbContextWrapper = null,
        IDynamicParametersWrapper dynamicParametersWrapper = null,
        IReadOnlyPolicyRegistry<string> policyRegistry = null,
        ILogger<SearchFilterRepository> logger = null)
    {
        dbContextWrapper ??= Substitute.For<IDbContextWrapper>();
        dynamicParametersWrapper ??= Substitute.For<IDynamicParametersWrapper>();
        policyRegistry ??= Substitute.For<IReadOnlyPolicyRegistry<string>>();
        logger ??= Substitute.For<ILogger<SearchFilterRepository>>();

        return new SearchFilterRepository(
            dbContextWrapper,
            dynamicParametersWrapper,
            policyRegistry,
            logger);
    }
}