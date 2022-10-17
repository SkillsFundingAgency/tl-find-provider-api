
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IEmployerInterestRepository
{
    Task<(int Count, Guid UniqueId)> Create(EmployerInterest employerInterest);

    Task<int> Delete(Guid uniqueId);

    Task<int> DeleteBefore(DateTime date);

    Task<EmployerInterest> Get(int id);

    Task<IEnumerable<EmployerInterest>> GetAll();

    Task<IEnumerable<EmployerInterestSummary>> GetSummaryList();
}