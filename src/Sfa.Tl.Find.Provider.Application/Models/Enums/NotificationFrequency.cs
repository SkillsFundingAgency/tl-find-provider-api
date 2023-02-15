using System.ComponentModel.DataAnnotations;

namespace Sfa.Tl.Find.Provider.Application.Models.Enums;

public enum NotificationFrequency
{
    [Display(Name = "Immediately")]
    Immediately = 1,
    [Display(Name = "Daily")]
    Daily = 2,
    [Display(Name = "Weekly")]
    Weekly = 3
}
