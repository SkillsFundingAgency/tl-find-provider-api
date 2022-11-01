namespace Sfa.Tl.Find.Provider.Infrastructure.Configuration;

public class EmployerInterestSettings
{
    public string? CleanupJobSchedule { get; set; }
    public string? EmployerSupportSiteUri { get; set; }
    public int RetentionDays { get; set; }
    public DateTime? ServiceStartDate { get; set; }
    public int SearchRadius { get; set; }
    public string? UnsubscribeEmployerUri { get; set; }
}