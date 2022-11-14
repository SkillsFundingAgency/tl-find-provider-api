using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Extensions;

public static class BusinessRuleExtensions
{
    public static bool IsAvailableAtDate(this short deliveryYear, DateTime today)
    {
        return deliveryYear < today.Year
               || (deliveryYear == today.Year && today.Month >= 9);
    }

    public static bool IsInterestExpiring(this EmployerInterestSummary employerInterest,
        DateTime today, 
        int retentionDays, 
        int numberOfDays = 7)
    {
        return employerInterest.InterestExpiryDate(retentionDays).AddDays(-numberOfDays) < today;
    }
    
    public static bool IsInterestNew(this EmployerInterestSummary employerInterest, 
        DateTime today, 
        int numberOfDays = 7,
        DateOnly? serviceStartDate = null)
    {
        return employerInterest.CreatedOn.Date > today.AddDays(-numberOfDays)
               && (serviceStartDate is null 
                   || employerInterest.CreatedOn.Date > serviceStartDate.Value.AddDays(numberOfDays - 1).ToDateTime(new TimeOnly(0, 0, 0)));
    }

    public static DateTime InterestExpiryDate(this EmployerInterestSummary employerInterest,
        int retentionDays)
    {
        return employerInterest.CreatedOn.AddDays(retentionDays).Date;
    }
}