namespace Sfa.Tl.Find.Provider.Infrastructure.Configuration;

public class ProviderSettings
{
    public string? ConnectSiteUri { get; set; }

    public int DefaultSearchRadius { get; set; }

    public int DefaultNotificationSearchRadius { get; set; }

    public string? NotificationEmailImmediateSchedule { get; set; }
    
    public string? NotificationEmailDailySchedule { get; set;}

    public string? NotificationEmailWeeklySchedule { get; set; }
    
    public string? SupportSiteAccessConnectHelpUri { get; set; }
}