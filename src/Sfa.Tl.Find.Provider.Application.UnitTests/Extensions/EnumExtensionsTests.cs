using System.ComponentModel.DataAnnotations;
using Sfa.Tl.Find.Provider.Application.Extensions;

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

    [Theory(DisplayName = nameof(EnumExtensions.GetCustomAttribute) + " Data Tests")]
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
}