namespace Sfa.Tl.Find.Provider.Infrastructure.Configuration;

public class ProviderSettings
{
    public string? ConnectSiteUri { get; set; }

    public int DefaultSearchRadius { get; set; }

    public string? NotificationEmailJobSchedule { get; set;}

    public int NotificationEmailDayOfWeek { get; set; }
}