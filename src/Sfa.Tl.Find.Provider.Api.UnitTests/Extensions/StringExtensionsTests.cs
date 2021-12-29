using FluentAssertions;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Extensions;

public class StringExtensionsTests
{
    [Theory(DisplayName = nameof(StringExtensions.FormatPostcodeForUri) + " Data Tests")]
    [InlineData("CV1 2WT", "CV1%202WT")]
    [InlineData("cv1 2wt", "CV1%202WT")]
    [InlineData(" CV1 2WT ", "CV1%202WT")]
    public void String_FormatPostcodeForUri_Data_Tests(string input, string expectedResult)
    {
        var result = input.FormatPostcodeForUri();
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(StringExtensions.ParseTLevelDefinitionName) + " Data Tests")]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData(" Introduction To Parsing ", "Introduction to Parsing")]
    [InlineData("T Level in Parsing ", "Parsing")]
    [InlineData("T Level In Parsing ", "Parsing")]
    [InlineData("T Level Education - Education and Childcare", "Education and Childcare")]
    [InlineData("T Level Education - Education and Childcare", "Education and Childcare")]
    [InlineData("T Level Education - Education and Childcare", "Education", 9)]
    [InlineData("T Level Education - Education and Childcare", "Education", 10)]
    public void String_ParseTLevelDefinitionName_Data_Tests(string input, string expectedResult, int maxLength = -1)
    {
        var result = maxLength < 0 
            ? input.ParseTLevelDefinitionName() 
            : input.ParseTLevelDefinitionName(maxLength);

        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(StringExtensions.ToTitleCase) + "Data Tests")]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("hello world", "Hello World")]
    [InlineData("ten Out Of 10", "Ten Out of 10")]
    [InlineData("Abingdon And Witney College", "Abingdon and Witney College")]
    [InlineData("Abingdon and Witney College", "Abingdon and Witney College")]
    [InlineData("abingdon and witney college", "Abingdon and Witney College")]
    [InlineData("Design, surveying and planning for Construction", "Design, Surveying and Planning for Construction")]
    [InlineData("Building services engineering for construction", "Building Services Engineering for Construction")]
    public void String_ToTitleCase_Data_Tests(string input, string expectedResult)
    {
        var result = input.ToTitleCase();

        result.Should().Be(expectedResult);
    }
}