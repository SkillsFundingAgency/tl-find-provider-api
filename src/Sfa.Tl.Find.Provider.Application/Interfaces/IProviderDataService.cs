using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IProviderDataService
{
    Task<ProviderSearchResponse> FindProviders(
        string searchTerms,
        IList<int> routeIds = null,
        IList<int> qualificationIds = null,
        int page = 0,
        int pageSize = Constants.DefaultPageSize);

    Task<ProviderSearchResponse> FindProviders(
        double latitude,
        double longitude,
        IList<int> routeIds = null,
        IList<int> qualificationIds = null,
        int page = 0,
        int pageSize = Constants.DefaultPageSize);

    Task<byte[]> GetCsv();
    
    Task<ProviderDetailResponse> GetAllProviders();

    Task<IEnumerable<Industry>> GetIndustries();

    Task<IEnumerable<LocationPostcode>> GetLocationPostcodes(long ukPrn);

    Task<IEnumerable<Qualification>> GetQualifications();

    Task<IEnumerable<Route>> GetRoutes();

    Task DeleteNotification(int notificationId);

    Task DeleteNotificationLocation(int notificationLocationId);

    Task<IEnumerable<NotificationSummary>> GetNotificationSummaryList(long ukPrn);

    Task<IEnumerable<NotificationLocationSummary>> GetNotificationLocationSummaryList(int notificationId);

    Task<Notification> GetNotification(int notificationId);

    Task<Notification> GetNotificationLocation(int notificationLocationId);

    Task<IEnumerable<NotificationLocationName>> GetAvailableNotificationLocationPostcodes(int providerNotificationId);

    Task<IEnumerable<SearchFilter>> GetSearchFilterSummaryList(long ukPrn);
    
    Task<SearchFilter> GetSearchFilter(int locationId);

    Task<bool> HasQualifications();

    Task<bool> HasProviders();
    
    Task ImportProviderContacts(Stream stream);

    Task ImportProviderData(Stream stream, bool isAdditionalData);

    Task SaveNotification(Notification notification, long ukPrn);

    Task SaveNotificationLocation(Notification notification, int? providerNotificationId = null);

    Task SaveSearchFilter(SearchFilter searchFilter);

    Task SendProviderNotifications();
    
    Task SendProviderNotificationEmail(int notificationId, string emailAddress);

    Task SendProviderVerificationEmail(int notificationId, string emailAddress);

    Task VerifyNotificationEmail(string token);
}