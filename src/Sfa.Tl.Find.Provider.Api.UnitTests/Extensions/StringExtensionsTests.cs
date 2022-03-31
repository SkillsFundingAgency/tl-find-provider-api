using FluentAssertions;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Models;
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

    [Theory(DisplayName = nameof(StringExtensions.FormatTownName) + " Data Tests")]
    [InlineData("Oxford", "Oxfordshire", "Oxfordshire", "Oxford, Oxfordshire")]
    [InlineData("Bristol", null, "Gloucestershire", "Bristol, Gloucestershire")]
    [InlineData("Coventry", "West Midlands", "West Midlands", "Coventry, West Midlands")]
    [InlineData("Coventry", "", "", "Coventry")]
    [InlineData("Some Town (Somewhere)", "Some County", null, "Some Town (Somewhere), Some County")]
    public void String_FormatTownName_Data_Tests(string name, string county, string localAuthority, string expectedResult)
    {
        var town = new Town
        {
            Name = name,
            County = county,
            LocalAuthority = localAuthority
        };

        var result = town.FormatTownName();
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(StringExtensions.IsPostcode) + " Data Tests")]
    [InlineData("CV1 2WT", true)]
    [InlineData("cv1 2wt", true)]
    [InlineData("OXX 9XX", false)]
    [InlineData("Cov", false)]
    [InlineData("Coventry", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void String_IsPostcode(string input, bool expectedResult)
    {
        var result = input.IsPostcode();
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(StringExtensions.IsPartialPostcode) + " Data Tests")]
    [InlineData("CV1 2WT", false)]
    [InlineData("CV1", true)]
    [InlineData("cv1", true)]
    [InlineData("L1", true)]
    [InlineData("OXX", false)]
    [InlineData("Cov", false)]
    [InlineData("Coventry", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void String_IsPartialPostcode(string input, bool expectedResult)
    {
        var result = input.IsPartialPostcode();
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(StringExtensions.IsFullOrPartialPostcode) + " Data Tests")]
    [InlineData("CV1 2WT", true)]
    [InlineData("cv1 2wt", true)]
    [InlineData("OXX 9XX", false)]
    [InlineData("CV1", true)]
    [InlineData("Cov", false)]
    [InlineData("Coventry", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("cv1", true)]
    [InlineData("L1", true)]
    [InlineData("OXX", false)]
    public void String_IsFullOrPartialPostcode(string input, bool expectedResult)
    {
        var result = input.IsFullOrPartialPostcode();
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
    [InlineData("Bob's burger's", "Bob's Burger's")]
    [InlineData("Bob’s burger’s", "Bob’s Burger’s")]
    public void String_ToTitleCase_Data_Tests(string input, string expectedResult)
    {
        var result = input.ToTitleCase();

        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(StringExtensions.ToSearchableString) + " Data Tests")]
    // ReSharper disable StringLiteralTypo
    [InlineData(null, null)]
    [InlineData("CV1 2WT", "cv12wt")]
    [InlineData("St. Albans", "stalbans")]
    [InlineData("Colton & the Ridwares", "coltonandtheridwares")]
    [InlineData("Coates (Cotswold),	Gloucestershire", "coatescotswoldgloucestershire")]
    [InlineData("Coleorton/Griffydam, Leicestershire", "coleortongriffydamleicestershire")]
    [InlineData("Collett's Green", "collettsgreen")]
    // ReSharper restore StringLiteralTypo
    public void String_ToSearchableString_Data_Tests(string input, string expectedResult)
    {
        var result = input.ToSearchableString();

        result.Should().Be(expectedResult);
    }
}