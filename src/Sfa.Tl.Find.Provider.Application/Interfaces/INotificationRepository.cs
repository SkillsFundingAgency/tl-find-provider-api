using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface INotificationRepository
{
    Task Delete(int notificationId);

    Task<IEnumerable<Notification>> GetNotifications(
        long ukPrn, 
        bool includeAdditionalData);

    Task<Notification> GetNotification(int locationId);

    Task Save(Notification notification);
}