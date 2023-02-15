using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Extensions;

public class BusinessRuleExtensionsTests
{
    [Theory(DisplayName = $"{nameof(BusinessRuleExtensions.IsAvailableAtDate)} Data Tests")]
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

    [Theory(DisplayName = $"{nameof(BusinessRuleExtensions.InterestExpiryDate)} Data Tests")]
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

        var result = target.InterestExpiryDate(retentionDays);
        result.Should().Be(expectedResultDate);
    }

    [Theory(DisplayName = $"{nameof(BusinessRuleExtensions.IsInterestExpiring)} Data Tests")]
    [InlineData("2022-12-08 23:59:59.999999", "2022-12-01", false)]
    [InlineData("2022-12-08 23:59:59.999999", "2022-12-02", true)]
    [InlineData("2022-12-08 23:59:59.999999", "2022-12-08", true)]
    public void EmployerInterestSummary_IsInterestExpiring_Data_Tests(string expiryDate, string currentDate, bool expectedResult)
    {
        var today = DateTime.Parse(currentDate);

        var target = new EmployerInterestSummary
        {
            ExpiryDate = DateTime.Parse(expiryDate)
        };

        var result = target.IsInterestExpiring(today);
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = $"{nameof(BusinessRuleExtensions.IsInterestNew)} Data Tests")]
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

    [Theory(DisplayName = $"{nameof(BusinessRuleExtensions.SetSummaryListFlags)} Data Tests")]
    [InlineData("2022-12-01 11:30", "2022-12-08 23:59:59.999999", "2022-12-01", true, false)]
    [InlineData("2022-12-01 11:30","2022-12-08 23:59:59.999999", "2022-12-02", true, true)]
    [InlineData("2022-12-01 11:30","2022-12-08 23:59:59.999999", "2022-12-08", false, true)]
    public void EmployerInterestSummary_SetSummaryListFlags_Data_Tests(string createdDate, string expiryDate, string currentDate, bool expectedIsNew, bool expectedIsExpiring)
    {
        var today = DateTime.Parse(currentDate);
        
        var target = new List<EmployerInterestSummary>
        {
            new()
            {
                OrganisationName = "Test",
                CreatedOn = DateTime.Parse(createdDate),
                ExpiryDate = DateTime.Parse(expiryDate)}
            };

        var result = target.SetSummaryListFlags(today).ToList();

        result.Should().NotBeNullOrEmpty();
        result.First().OrganisationName.Should().Be(target.First().OrganisationName);
        result.First().IsNew.Should().Be(expectedIsNew);
        result.First().IsExpiring.Should().Be(expectedIsExpiring);
    }
}