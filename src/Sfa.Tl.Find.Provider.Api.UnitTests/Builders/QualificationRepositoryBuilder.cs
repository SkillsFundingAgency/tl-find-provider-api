using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    public class QualificationRepositoryBuilder
    {
        public QualificationRepository Build(
            IDbContextWrapper dbContextWrapper = null)
        {
            dbContextWrapper ??= Substitute.For<IDbContextWrapper>();

            return new QualificationRepository(dbContextWrapper);
        }
    }
}
