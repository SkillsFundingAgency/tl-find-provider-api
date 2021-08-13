using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    public class ProviderRepositoryBuilder
    {
        public ProviderRepository Build(
            IDbContextWrapper dbContextWrapper = null,
            ILogger<ProviderRepository> logger = null)
        {
            dbContextWrapper ??= Substitute.For<IDbContextWrapper>();
            logger ??= Substitute.For<ILogger<ProviderRepository>>();
                
            return new ProviderRepository(dbContextWrapper, logger);
        }
    }
}
