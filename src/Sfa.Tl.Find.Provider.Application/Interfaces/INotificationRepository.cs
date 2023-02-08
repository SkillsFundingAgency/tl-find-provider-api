using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface INotificationRepository
{
    Task<int> Create(Notification notification, long ukPrn);

    Task CreateLocation(Notification notification, int providerNotificationId);

    Task Delete(int providerNotificationId);

    Task DeleteLocation(int notificationLocationId);

    Task<IEnumerable<NotificationLocationName>> GetProviderNotificationLocations(int providerNotificationId);

    Task<Notification> GetNotification(int notificationId);

    Task<IEnumerable<NotificationEmail>> GetPendingNotificationEmails(NotificationFrequency frequency);

    Task<Notification> GetNotificationLocation(int notificationLocationId);

    Task<IEnumerable<NotificationSummary>> GetNotificationSummaryList(
        long ukPrn,
        bool includeAdditionalData);

    Task<IEnumerable<NotificationLocationSummary>> GetNotificationLocationSummaryList(
        int notificationId);

    Task Update(Notification notification);

    Task UpdateLocation(Notification notification);

    Task UpdateNotificationSentDate(int notificationLocationId);

    Task SaveEmailVerificationToken(int notificationId, string emailAddress, Guid? emailVerificationToken);

    Task<(bool Success, string Email)> VerifyEmailToken(Guid emailVerificationToken);
}