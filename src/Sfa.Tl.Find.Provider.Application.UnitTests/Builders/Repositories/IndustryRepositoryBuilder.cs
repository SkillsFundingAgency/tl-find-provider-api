using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;

public class IndustryRepositoryBuilder
{
    public IIndustryRepository Build(
        IDbContextWrapper dbContextWrapper = null)
    {
        dbContextWrapper ??= Substitute.For<IDbContextWrapper>();
        
        return new IndustryRepository(
            dbContextWrapper);
    }
}