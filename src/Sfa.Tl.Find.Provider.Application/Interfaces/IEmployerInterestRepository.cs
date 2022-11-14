
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IEmployerInterestRepository
{
    Task<(int Count, Guid UniqueId)> Create(
        EmployerInterest employerInterest,
        GeoLocation geoLocation);

    Task<int> Delete(Guid uniqueId);

    Task<int> DeleteBefore(DateTime date);

    Task<EmployerInterestDetail> GetDetail(int id);

    Task<IEnumerable<EmployerInterest>> GetAll();

    Task<IEnumerable<EmployerInterestSummary>> GetSummaryList();

    Task<(IEnumerable<EmployerInterestSummary> SearchResults, int TotalResultsCount)> Search(
        double latitude, 
        double longitude, 
        int searchRadius);
}