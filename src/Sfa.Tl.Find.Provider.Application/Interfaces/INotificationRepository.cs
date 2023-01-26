using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface INotificationRepository
{
    Task Delete(int notificationId);

    Task<Notification> GetNotification(int notificationId);

    Task<IEnumerable<NotificationSummary>> GetNotificationSummaryList(
        long ukPrn, 
        bool includeAdditionalData);

    Task<IEnumerable<NotificationLocationSummary>> GetNotificationLocationSummaryList(
        int notificationId);

    Task Create(Notification notification, long ukPrn);

    Task Update(Notification notification);

    Task SaveEmailVerificationToken(int notificationId, string emailAddress, Guid? verificationToken);

    Task RemoveEmailVerificationToken(Guid verificationToken);
}