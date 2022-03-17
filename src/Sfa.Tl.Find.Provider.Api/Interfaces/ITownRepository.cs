using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Interfaces;

public interface ITownRepository
{
    Task<bool> HasAny();

    Task Save(IEnumerable<Town> qualifications);

    Task<IEnumerable<Town>> Search(string searchTerm, int maxResults);
}