using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Extensions;

public static class BusinessRuleExtensions
{
    public static bool IsAvailableAtDate(this short deliveryYear, DateTime today) =>
        deliveryYear < today.Year
        || (deliveryYear == today.Year && today.Month >= 9);

    public static bool IsInterestExpiring(this EmployerInterestSummary employerInterest,
        DateTime today,
        int numberOfDays = 7) =>
        employerInterest.ExpiryDate != null && employerInterest.ExpiryDate.Value.AddDays(-numberOfDays) < today;

    public static bool IsInterestNew(this EmployerInterestSummary employerInterest,
        DateTime today,
        int numberOfDays = 7) =>
        employerInterest.CreatedOn.Date > today.AddDays(-numberOfDays);

    public static DateTime InterestExpiryDate(this EmployerInterestSummary employerInterest,
        int retentionDays) =>
        employerInterest.CreatedOn.AddDays(retentionDays).Date;

    public static IEnumerable<EmployerInterestSummary> SetSummaryListFlags(this IEnumerable<EmployerInterestSummary> employerInterestSummary,
        DateTime today)
    {
        return employerInterestSummary
            .Select(x =>
            {
                x.IsNew = x.IsInterestNew(today);
                x.IsExpiring = x.IsInterestExpiring(today);
                return x;
            });
    }
}