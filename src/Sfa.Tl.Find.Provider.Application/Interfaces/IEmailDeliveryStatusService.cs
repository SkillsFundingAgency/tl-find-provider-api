using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;
public interface IEmailDeliveryStatusService
{
    Task<int> HandleEmailDeliveryStatus(EmailDeliveryReceipt deliveryReceipt);
}
