using System.ComponentModel.DataAnnotations;

namespace Sfa.Tl.Find.Provider.Application.Models.Enums;

public enum ContactPreference
{
    [Display(Name = "Email")]
    Email = 1,
    [Display(Name = "Telephone")]
    Telephone = 2,
    [Display(Name = "No preference")]
    NoPreference = 3
}
