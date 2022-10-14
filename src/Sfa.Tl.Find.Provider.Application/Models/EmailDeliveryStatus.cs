using System.ComponentModel;

namespace Sfa.Tl.Find.Provider.Application.Models;
public static class EmailDeliveryStatus
{
    public const string Delivered = "delivered";

    [Description("Email address does not exist")]
    public const string PermanentFailure = "permanent-failure";

    [Description("Inbox not accepting messages right now")]
    public const string TemporaryFailure = "temporary-failure";

    [Description("Problem between Notify and the provider")]
    public const string TechnicalFailure = "technical-failure";
}
