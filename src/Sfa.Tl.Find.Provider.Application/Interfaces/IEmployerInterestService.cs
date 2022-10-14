using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;
public interface IEmployerInterestService
{
    Task<Guid> CreateEmployerInterest(EmployerInterest employerInterest);

    Task<int> DeleteEmployerInterest(Guid uniqueId);

    Task<int> RemoveExpiredEmployerInterest();

    Task<IEnumerable<EmployerInterestSummaryItem>> FindEmployerInterest();

    Task<EmployerInterest> GetEmployerInterest(int id);

    int RetentionDays { get; }
}
