using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Interfaces;

public interface IRouteRepository
{
    Task<IEnumerable<Route>> GetAll(bool includeAdditionalData);
}