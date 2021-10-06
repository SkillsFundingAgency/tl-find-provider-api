using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    public class RouteRepositoryBuilder
    {
        public RouteRepository Build(
            IDbContextWrapper dbContextWrapper = null,
            ILogger<RouteRepository> logger = null)
        {
            dbContextWrapper ??= Substitute.For<IDbContextWrapper>();
            logger ??= Substitute.For<ILogger<RouteRepository>>();

            return new RouteRepository(dbContextWrapper, logger);
        }
    }
}
