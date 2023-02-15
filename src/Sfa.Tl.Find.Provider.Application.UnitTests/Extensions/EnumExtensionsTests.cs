using System.ComponentModel.DataAnnotations;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Models.Enums;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Extensions;

public class EnumExtensionsTests
{

    public enum TestEnum
    {
        [Display(Name = "Test 1", Description = "Test 1 Desc")]
        Test1,
        [Display(Description = "Test 2 Desc")]
        Test2,
        Test3
    }

    [Theory(DisplayName = $"{nameof(EnumExtensions.GetCustomAttribute)} Data Tests")]
    [InlineData(TestEnum.Test1, true, "Test 1", "Test 1 Desc")]
    [InlineData(TestEnum.Test2, true, null, "Test 2 Desc")]
    [InlineData(TestEnum.Test3, false, null, null)]
    public void GetCustomAttribute_DisplayAttribute_Data_Tests(
        TestEnum input,
        bool shouldHaveValue,
        string expectedName,
        string expectedDescription)
    {
        var result = input.GetCustomAttribute<DisplayAttribute>();
        if (shouldHaveValue)
        {
            result.Should().NotBeNull();
            result.Name.Should().Be(expectedName);
            result.Description.Should().Be(expectedDescription);
        }
        else
        {
            result.Should().BeNull();
        }
    }

    [Theory(DisplayName = $"{nameof(EnumExtensions.GetEnumDisplayName)} {nameof(ContactPreference)} Data Tests")]
    [InlineData(null, "")]
    [InlineData(ContactPreference.Email, "Email")]
    [InlineData(ContactPreference.Telephone, "Telephone")]
    [InlineData(ContactPreference.NoPreference, "No preference")]
    [InlineData((ContactPreference)4, "Unknown")]
    [InlineData((ContactPreference)5, "No idea", "No idea")]
    public void GetEnumDisplayName_ContactPreference_Data_Tests(ContactPreference? contactPreference, string expectedResult, string defaultValue = null)
    {
        var result = defaultValue is null
            ? contactPreference.GetEnumDisplayName()
            : contactPreference.GetEnumDisplayName(defaultValue);

        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = $"{nameof(EnumExtensions.GetEnumDisplayName)} {nameof(NotificationFrequency)} Data Tests")]
    [InlineData(NotificationFrequency.Immediately, "Immediately")]
    [InlineData(NotificationFrequency.Daily, "Daily")]
    [InlineData(NotificationFrequency.Weekly, "Weekly")]
    public void GetEnumDisplayName_NotificationFrequency_Data_Tests(NotificationFrequency frequency, string expectedResult)
    {
        var result = frequency.GetEnumDisplayName();

        result.Should().Be(expectedResult);
    }
}