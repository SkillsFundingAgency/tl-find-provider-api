using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface INotificationRepository
{
    Task Delete(int providerNotificationId);
    
    Task DeleteLocation(int notificationLocationId);

    Task<Notification> GetNotification(int notificationId);

    Task<Notification> GetNotificationLocation(int notificationLocationId);

    Task<IEnumerable<NotificationSummary>> GetNotificationSummaryList(
        long ukPrn, 
        bool includeAdditionalData);

    Task<IEnumerable<NotificationLocationSummary>> GetNotificationLocationSummaryList(
        int notificationId);

    Task Create(Notification notification, long ukPrn);

    Task CreateLocation(Notification notification, int providerNotificationId);

    Task Update(Notification notification);

    Task UpdateLocation(Notification notification);

    Task SaveEmailVerificationToken(int notificationId, string emailAddress, Guid? emailVerificationToken);

    Task RemoveEmailVerificationToken(Guid emailVerificationToken);
}