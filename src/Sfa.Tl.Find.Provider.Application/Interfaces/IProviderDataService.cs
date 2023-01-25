﻿using Sfa.Tl.Find.Provider.Application.Models;

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

    Task<IEnumerable<NotificationSummary>> GetNotificationSummaryList(long ukPrn);
    
    Task<Notification> GetNotification(int notificationId);

    Task<IEnumerable<SearchFilter>> GetSearchFilterSummaryList(long ukPrn);
    
    Task<SearchFilter> GetSearchFilter(int locationId);

    Task<bool> HasQualifications();

    Task<bool> HasProviders();
    
    Task ImportProviderContacts(Stream stream);

    Task ImportProviderData(Stream stream, bool isAdditionalData);

    Task SaveNotification(Notification notification, long ukPrn);

    Task SaveSearchFilter(SearchFilter searchFilter);
    
    Task SendProviderVerificationEmail(int notificationId, string emailAddress);

    Task SendProviderNotificationEmail(int notificationId, string emailAddress);

    Task VerifyNotificationEmail(string token);
}