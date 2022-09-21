using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;
public interface IEmployerInterestService
{
    Task<int> DeleteEmployerInterest(Guid uniqueId);

    Task<int> RemoveExpiredEmployerInterest();

    Task<IEnumerable<EmployerInterest>> FindEmployerInterest();
}
