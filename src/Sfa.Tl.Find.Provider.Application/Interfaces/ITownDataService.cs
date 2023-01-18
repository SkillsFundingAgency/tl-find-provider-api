using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface ITownDataService
{
    Task<bool> HasTowns();

    Task ImportTowns();

    Task ImportTowns(Stream stream);

    Task<IEnumerable<Town>> Search(string searchTerm, int maxResults = Constants.TownSearchDefaultMaxResults);
}