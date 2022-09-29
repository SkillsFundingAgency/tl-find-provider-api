using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.Services;
public class GuidService : IGuidService
{
    public Guid NewGuid() =>
        new Guid();

}
