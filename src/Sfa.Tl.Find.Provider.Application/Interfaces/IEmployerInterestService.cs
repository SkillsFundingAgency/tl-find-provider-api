using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;
public interface IEmployerInterestService
{
    Task<Guid> CreateEmployerInterest(EmployerInterest employerInterest);

    Task<int> DeleteEmployerInterest(Guid uniqueId);

    Task<int> RemoveExpiredEmployerInterest();

    Task<IEnumerable<EmployerInterestSummary>> FindEmployerInterest();

    Task<EmployerInterestDetail> GetEmployerInterestDetail(int id);

    int RetentionDays { get; }
}
