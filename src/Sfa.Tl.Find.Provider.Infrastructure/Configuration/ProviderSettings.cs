namespace Sfa.Tl.Find.Provider.Infrastructure.Configuration;

public class ProviderSettings
{
    public string? ConnectSiteUri { get; set; }

    public int DefaultSearchRadius { get; set; }

    public int DefaultNotificationSearchRadius { get; set; }

    public string? NotificationEmailJobSchedule { get; set;}

    public string? NotificationEmailImmediateJobSchedule { get; set;}

    public int NotificationEmailWeeklyDay { get; set; }

    public string? SupportSiteAccessConnectHelpUri { get; set; }
}