using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Extensions;

public class BusinessRuleExtensionsTests
{
    [Theory(DisplayName = nameof(BusinessRuleExtensions.IsAvailableAtDate) + " Data Tests")]
    [InlineData(2020, "2020-12-31", true)]
    [InlineData(2021, "2020-12-31", false)]
    [InlineData(2021, "2021-08-31", false)]
    [InlineData(2021, "2021-09-01", true)]
    [InlineData(2021, "2022-09-01", true)]
    [InlineData(2022, "2022-08-31", false)]
    [InlineData(2022, "2022-09-01", true)]
    [InlineData(2023, "2023-08-31", false)]
    [InlineData(2023, "2023-09-01", true)]
    public void DeliveryYear_IsAvailableAtDate_Data_Tests(short deliveryYear, string currentDate, bool expectedResult)
    {
        var today = DateTime.Parse(currentDate);

        var result = deliveryYear.IsAvailableAtDate(today);
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(BusinessRuleExtensions.InterestExpiryDate) + " Data Tests")]
    [InlineData("2022-12-01 11:30", null, 30, "2022-12-31")]
    [InlineData("2022-12-01 11:30", null, 84, "2023-02-23")]
    public void EmployerInterestSummary_InterestExpiryDate_Data_Tests(string createdDate, string modifiedDate, int retentionDays, string expectedResult)
    {
        var expectedResultDate = DateTime.Parse(expectedResult);

        var target = new EmployerInterestSummary
        {
            CreatedOn = DateTime.Parse(createdDate),
            ModifiedOn = modifiedDate is not null ? DateTime.Parse(modifiedDate) : null
        };
        //Expected result to be
        //<2022 - 12 - 31 >, but found
        //<2020 - 12 - 31 11:30:00 >.

        var result = target.InterestExpiryDate(retentionDays);
        result.Should().Be(expectedResultDate);
    }

    [Theory(DisplayName = nameof(BusinessRuleExtensions.IsInterestExpiring) + " Data Tests")]
    [InlineData("2022-12-01 11:30", null, "2022-12-01", 30, false)]
    [InlineData("2022-12-01 11:30", null, "2022-12-24", 30, false)]
    [InlineData("2022-12-01 11:30", null, "2022-12-25", 30, true)]
    public void EmployerInterestSummary_IsInterestExpiring_Data_Tests(string createdDate, string modifiedDate, string currentDate, int daysToRetain, bool expectedResult)
    {
        var today = DateTime.Parse(currentDate);

        var target = new EmployerInterestSummary
        {
            CreatedOn = DateTime.Parse(createdDate),
            ModifiedOn = modifiedDate is not null ? DateTime.Parse(modifiedDate) : null
        };

        var result = target.IsInterestExpiring(today, daysToRetain);
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(BusinessRuleExtensions.IsInterestNew) + " Data Tests")]
    [InlineData("2022-12-01 11:30", null, "2022-12-01", true)]
    [InlineData("2022-12-01 11:30", null, "2022-12-07", true)]
    [InlineData("2022-12-01 11:30", null, "2022-12-08", false)]
    [InlineData("2022-12-01 11:30", null, "2022-12-09", false)]
    public void EmployerInterestSummary_IsInterestNew_Data_Tests(string createdDate, string modifiedDate, string currentDate, bool expectedResult)
    {
        var today = DateTime.Parse(currentDate);

        var target = new EmployerInterestSummary
        {
            CreatedOn = DateTime.Parse(createdDate),
            ModifiedOn = modifiedDate is not null ? DateTime.Parse(modifiedDate) : null
        };

        var result = target.IsInterestNew(today);
        result.Should().Be(expectedResult);
    }
}