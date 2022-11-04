using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Extensions;

public static class BusinessRuleExtensions
{
    public static bool IsAvailableAtDate(this short deliveryYear, DateTime today)
    {
        return deliveryYear < today.Year
               || (deliveryYear == today.Year && today.Month >= 9);
    }

    /*
                    var availableUntil = employerInterest.CreatedOn.AddDays(Model.EmployerInterestRetentionDays);
                        @*
                            ‘New’ tag shows next to organisation name if the interest has been received in the last 7 days.
                            ‘Expiring’ tag shows next to organisation name if the interest expires in the next 7 days.
                        *@
                        var isExpiring = availableUntil.AddDays(-7) < DateTime.Today;
                        var isNew = employerInterest.CreatedOn.AddDays(-7) < DateTime.Today.AddDays(-7);
     */
    public static bool IsInterestExpiring(this EmployerInterestSummary employerInterest,
        DateTime today, 
        int retentionDays, 
        int numberOfDays = 7)
    {
        return employerInterest.InterestExpiryDate(retentionDays).AddDays(-numberOfDays) < today;
    }

    public static bool IsInterestNew(this EmployerInterestSummary employerInterest, 
        DateTime today, 
        int numberOfDays = 7)
    {
        return employerInterest.CreatedOn.Date > today.AddDays(-numberOfDays);
    }

    public static DateTime InterestExpiryDate(this EmployerInterestSummary employerInterest,
        int retentionDays)
    {
        return employerInterest.CreatedOn.AddDays(retentionDays).Date;
    }
}