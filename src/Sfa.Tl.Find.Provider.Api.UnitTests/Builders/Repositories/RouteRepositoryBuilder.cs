using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Repositories;

public class RouteRepositoryBuilder
{
    public RouteRepository Build(
        IDbContextWrapper dbContextWrapper = null,
        IDynamicParametersWrapper dynamicParametersWrapper = null)
    {
        dbContextWrapper ??= Substitute.For<IDbContextWrapper>();
        dynamicParametersWrapper ??= Substitute.For<IDynamicParametersWrapper>();
        
        return new RouteRepository(
            dbContextWrapper,
            dynamicParametersWrapper);
    }
}