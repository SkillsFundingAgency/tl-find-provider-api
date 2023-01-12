using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetNotifications(
        long ukPrn, 
        bool includeAdditionalData);
}