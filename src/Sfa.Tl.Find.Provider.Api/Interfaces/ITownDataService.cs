using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Interfaces;

public interface ITownDataService
{
    Task<bool> HasTowns();

    Task ImportTowns();

    Task<IEnumerable<Town>> Search(string searchString, int maxResults = Constants.TownSearchDefaultMaxResults);
}