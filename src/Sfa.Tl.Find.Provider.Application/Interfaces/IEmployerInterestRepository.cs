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

    Task<IEnumerable<ExpiredEmployerInterestDto>> DeleteExpired(DateTime date);

    Task<bool> ExtendExpiry(
        Guid uniqueId,
        int numberOfDaysToExtend, 
        int expiryNotificationDays, 
        int maximumExtensions);

    Task<EmployerInterestDetail> GetDetail(int id);

    Task<IEnumerable<EmployerInterest>> GetExpiringInterest(int daysToExpiry);

    Task<IEnumerable<EmployerInterestSummary>> GetSummaryList();

    Task<(IEnumerable<EmployerInterestSummary> SearchResults, int TotalResultsCount)> Search(
        double latitude,
        double longitude,
        int searchRadius);

    Task<(IEnumerable<EmployerInterestSummary> SearchResults, int TotalResultsCount, bool SearchFiltersApplied)> Search(
        int locationId,
        int defaultSearchRadius);

    Task UpdateExtensionEmailSentDate(int id);
}