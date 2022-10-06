namespace Sfa.Tl.Find.Provider.Application.Models.Configuration;

public class EmployerInterestSettings
{
    public string CleanupJobSchedule { get; set; }
    public string EmployerSupportSiteUri { get; set; }
    public int RetentionDays { get; set; }
    public string UnsubscribeEmployerUri { get; set; }
}