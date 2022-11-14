using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Infrastructure.Services;
public class GuidService : IGuidService
{
    public Guid NewGuid() =>
        Guid.NewGuid();
}
