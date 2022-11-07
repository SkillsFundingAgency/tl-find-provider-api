using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;
public interface IEmployerInterestService
{
    int RetentionDays { get; }

    DateOnly? ServiceStartDate { get; }

    Task<Guid> CreateEmployerInterest(EmployerInterest employerInterest);

    Task<int> DeleteEmployerInterest(Guid uniqueId);

    Task<int> RemoveExpiredEmployerInterest();

    Task<(IEnumerable<EmployerInterestSummary> SearchResults, int TotalResultsCount)> FindEmployerInterest(
        double latitude,
        double longitude);

    Task<(IEnumerable<EmployerInterestSummary> SearchResults, int TotalResultsCount)> 
        FindEmployerInterest(string postcode);

    Task<EmployerInterestDetail> GetEmployerInterestDetail(int id);

    Task<IEnumerable<EmployerInterestSummary>> GetSummaryList();
}
