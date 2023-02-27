using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;
public interface INotificationService
{
    Task<int> CreateNotification(Notification notification, long ukPrn);

    Task CreateNotificationLocation(Notification notification, int? providerNotificationId);

    Task DeleteNotification(int notificationId);

    Task DeleteNotificationLocation(int notificationLocationId);

    Task<IEnumerable<NotificationSummary>> GetNotificationSummaryList(long ukPrn);

    Task<IEnumerable<NotificationLocationSummary>> GetNotificationLocationSummaryList(int notificationId);

    Task<Notification> GetNotification(int notificationId);

    Task<Notification> GetNotificationLocation(int notificationLocationId);

    Task<IEnumerable<NotificationLocationName>> GetAvailableNotificationLocationPostcodes(int providerNotificationId);

    Task SendProviderNotifications(NotificationFrequency frequency);

    Task SendProviderNotificationEmail(string emailAddress);

    Task SendProviderNotificationVerificationEmail(int notificationId, string emailAddress);

    Task UpdateNotification(Notification notification);

    Task UpdateNotificationLocation(Notification notification);

    Task<(bool Success, string Email)> VerifyNotificationEmail(string token);
}
