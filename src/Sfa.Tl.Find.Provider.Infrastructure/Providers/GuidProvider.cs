using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Infrastructure.Providers;
public class GuidProvider : IGuidProvider
{
    public Guid NewGuid() =>
        Guid.NewGuid();
}
