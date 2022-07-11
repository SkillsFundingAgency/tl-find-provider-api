using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface ITownRepository
{
    Task<bool> HasAny();

    Task Save(IEnumerable<Town> qualifications);

    Task<IEnumerable<Town>> Search(string searchTerms, int maxResults);
}