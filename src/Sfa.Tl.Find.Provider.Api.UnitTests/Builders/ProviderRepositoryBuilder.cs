using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    public class ProviderRepositoryBuilder
    {
        public ProviderRepository Build(
            IDbContextWrapper dbContextWrapper = null)
        {
            dbContextWrapper ??= Substitute.For<IDbContextWrapper>();

            return new ProviderRepository(dbContextWrapper);
        }
    }
}
