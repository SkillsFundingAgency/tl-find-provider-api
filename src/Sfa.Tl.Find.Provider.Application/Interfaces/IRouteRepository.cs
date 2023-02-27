using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IRouteRepository
{
    Task<IEnumerable<Route>> GetAll();
}