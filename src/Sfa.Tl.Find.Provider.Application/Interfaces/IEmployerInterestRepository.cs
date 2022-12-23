
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IEmployerInterestRepository
{
    Task<(int Count, Guid UniqueId)> Create(
        EmployerInterest employerInterest,
        GeoLocation geoLocation,
        DateTime expiryDate);

    Task<int> Delete(int id);

    Task<int> Delete(Guid uniqueId);

    Task<int> DeleteBefore(DateTime date);

    Task<bool> ExtendExpiry(Guid uniqueId, int numberOfDays);

    Task<IEnumerable<EmployerInterest>> GetAll();
    
    Task<EmployerInterestDetail> GetDetail(int id);

    Task<IEnumerable<EmployerInterest>> GetExpiringInterest(DateTime date);

    Task<IEnumerable<EmployerInterestSummary>> GetSummaryList();

    Task<(IEnumerable<EmployerInterestSummary> SearchResults, int TotalResultsCount)> Search(
        double latitude, 
        double longitude, 
        int searchRadius);

    Task UpdateExtensionEmailSentDate(int id);
}