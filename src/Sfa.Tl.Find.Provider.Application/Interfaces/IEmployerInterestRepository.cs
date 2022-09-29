
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IEmployerInterestRepository
{
    Task<(int Count, Guid UniqueId)> Create(EmployerInterest employerInterest);

    Task<int> Delete(Guid uniqueId);

    Task<int> DeleteBefore(DateTime date);

    Task<IEnumerable<EmployerInterest>> GetAll();
}