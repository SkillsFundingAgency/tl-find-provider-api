using Intertech.Facade.DapperParameters;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Repositories;

public class RouteRepositoryBuilder
{
    public RouteRepository Build(
        IDbContextWrapper dbContextWrapper = null,
        IDapperParameters dbParameters = null)
    {
        dbContextWrapper ??= Substitute.For<IDbContextWrapper>();
        dbParameters ??= Substitute.For<IDapperParameters>();
        
        return new RouteRepository(
            dbContextWrapper,
            dbParameters);
    }
}