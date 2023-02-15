using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;

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

    Task<bool> HasQualifications();

    Task<bool> HasProviders();
    
    Task ImportProviderContacts(Stream stream);

    Task ImportProviderData(Stream stream, bool isAdditionalData);

    Task SendProviderNotifications(NotificationFrequency frequency);
    
    Task SendProviderNotificationEmail(string emailAddress);

    Task SendProviderVerificationEmail(int notificationId, string emailAddress);

    Task<(bool Success, string Email)> VerifyNotificationEmail(string token);
}