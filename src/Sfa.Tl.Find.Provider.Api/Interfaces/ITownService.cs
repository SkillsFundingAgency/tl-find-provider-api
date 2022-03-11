using System.Threading.Tasks;

namespace Sfa.Tl.Find.Provider.Api.Interfaces;

public interface ITownDataService
{
    Task<bool> HasTowns();

    Task ImportTowns();
}