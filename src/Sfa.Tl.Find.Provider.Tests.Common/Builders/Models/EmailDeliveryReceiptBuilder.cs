using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
public class EmailDeliveryReceiptBuilder
{
    private string _status = EmailDeliveryStatus.Delivered;

    public  EmailDeliveryReceipt Build() =>
        new()
        {
            Id = Guid.Parse("c7d1930c-017c-45a0-b6b2-6479a4dcb75e"),
            CreatedAt = DateTime.Parse("2022-09-19 14:15:00"),
            CompletedAt = DateTime.Parse("2022-09-19 16:29:15"),
            SentAt = DateTime.Parse("2022-09-19 15:31:12"),
            To = "receiver@test.co.uk",
            NotificationType = "email",
            TemplateId = Guid.Parse("102e85fe-00e4-46cf-bb0c-8f43d28aead2"),
            Reference = "Test reference",
            Status = _status
        };

    public EmailDeliveryReceiptBuilder WithDeliveryStatus(string status)
    {
        _status = status;

        return this;
    }
}
