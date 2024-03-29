﻿using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Extensions;

public class StringExtensionsTests
{
    [Theory(DisplayName = $"{nameof(StringExtensions.IsPostcode)} Data Tests")]
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

    [Theory(DisplayName = $"{nameof(StringExtensions.IsPartialPostcode)} Data Tests")]
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

    [Theory(DisplayName = $"{nameof(StringExtensions.DoesNotMatch)} Data Tests")]
    [InlineData("/", Constants.CssPathPattern, true)]
    [InlineData("/Index", Constants.CssPathPattern, true)]
    [InlineData("/Folder/Index", Constants.CssPathPattern, true)]
    [InlineData("/Folder/Index", Constants.JsPathPattern, true)]
    [InlineData("/Folder/Index", Constants.FontsPathPattern, true)]
    [InlineData("/css/test.css", Constants.CssPathPattern, false)]
    [InlineData("/js/test.js", Constants.JsPathPattern, false)]
    [InlineData("/assets/fonts/bold-a123d-v2.woff", Constants.FontsPathPattern, false)]
    [InlineData("/assets/fonts/bold-a123d-v2.woff2", Constants.FontsPathPattern, false)]
    public void String_DoesNotMatch_Data_Tests(string input, string pattern, bool expectedResult)
    {
        var result = input.DoesNotMatch(pattern);
        result.Should().Be(expectedResult);
    }
    
    [Theory(DisplayName = $"{nameof(StringExtensions.IsFullOrPartialPostcode)} Data Tests")]
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

    [Theory(DisplayName = $"{nameof(StringExtensions.ParseTLevelDefinitionName)} Data Tests")]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData(" Introduction To Parsing ", "Introduction to Parsing")]
    [InlineData("T Level in Parsing ", "Parsing")]
    [InlineData("T Level In Parsing ", "Parsing")]
    [InlineData("T Level Education - Education and Early Years", "Education and Early Years")]
    [InlineData("T Level Education - Education and Early Years", "Education", 9)]
    [InlineData("T Level Education - Education and Early Years", "Education", 10)]
    public void String_ParseTLevelDefinitionName_Data_Tests(string input, string expectedResult, int maxLength = -1)
    {
        var result = maxLength < 0
            ? input.ParseTLevelDefinitionName()
            : input.ParseTLevelDefinitionName(maxLength);

        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = $"{nameof(StringExtensions.ToTitleCase)} Data Tests")]
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

    [Theory(DisplayName = $"{nameof(StringExtensions.ToSearchableString)} Data Tests")]
    // ReSharper disable StringLiteralTypo
    [InlineData(null, null)]
    [InlineData("CV1 2WT", "cv12wt")]
    [InlineData("St. Albans", "stalbans")]
    [InlineData("Colton & the Ridwares", "coltonandtheridwares")]
    [InlineData("Coates (Cotswold),	Gloucestershire", "coatescotswoldgloucestershire")]
    [InlineData("Coleorton/Griffydam, Leicestershire", "coleortongriffydamleicestershire")]
    [InlineData("Collett's Green", "collettsgreen")]
    [InlineData("Newcastle-under-Lyme, Staffordshire", "newcastleunderlymestaffordshire")]
    [InlineData("Westward Ho!", "westwardho")]
    [InlineData("Oakthorpe & Donisthorpe", "oakthorpeanddonisthorpe")]
    [InlineData("Bede, Tyne & Wear", "bedetyneandwear")]
    [InlineData("Bishop's Castle, Shropshire", "bishopscastleshropshire")]
    // ReSharper restore StringLiteralTypo
    public void String_ToSearchableString_Data_Tests(string input, string expectedResult)
    {
        var result = input.ToSearchableString();

        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = $"{nameof(StringExtensions.ReplaceBreaksWithNewlines)} Data Tests")]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("<br />", "\n")]
    [InlineData("hello<br />world", "hello\nworld")]
    [InlineData("hello<br/>world", "hello\nworld")]
    [InlineData("hello<br>world", "hello\nworld")]
    public void String_ReplaceBreaksWithNewlines_Data_Tests(string input, string expectedResult)
    {
        var result = input.ReplaceBreaksWithNewlines();
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = $"{nameof(StringExtensions.ReplaceMultipleLineBreaks)} Data Tests")]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("hello world", "hello world")]
    [InlineData("hello\nworld", "hello\nworld")]
    [InlineData("hello\n\nworld", "hello\nworld")]
    [InlineData("hello\n\n\nworld", "hello\nworld")]
    public void ReplaceMultipleLineBreaks(string input, string expectedResult)
    {
        var result = input.ReplaceMultipleLineBreaks();
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = $"{nameof(StringExtensions.ReplaceRedactedHttpStrings)} Data Tests")]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("http___www.test.com/", "http://www.test.com/")]
    [InlineData("https___www.test.com/", "https://www.test.com/")]
    public void String_ReplaceHttpsRedactor_Data_Tests(string input, string expectedResult)
    {
        var result = input.ReplaceRedactedHttpStrings();
        result.Should().Be(expectedResult);
    }
    
    [Theory(DisplayName = $"{nameof(StringExtensions.ToTrimmedOrNullString)} Data Tests")]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData(" ", null)]
    [InlineData("Test", "Test")]
    [InlineData(" Test ", "Test")]
    public void String_ToTrimmedOrNullString_Data_Tests(string input, string expectedResult)
    {
        var result = input.ToTrimmedOrNullString();
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = $"{nameof(StringExtensions.Truncate)} Data Tests")]
    [InlineData(null, 10, null)]
    [InlineData("", 10, "")]
    [InlineData(" ", 10, " ")]
    [InlineData("Test", 3, "Tes")]
    [InlineData("Test", 4, "Test")]
    [InlineData("Test", 5, "Test")]
    public void String_Truncate_Data_Tests(string input, int length, string expectedResult)
    {
        var result = input.Truncate(length);
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = $"{nameof(StringExtensions.Truncate)} Data Tests")]
    // ReSharper disable StringLiteralTypo
    [InlineData(null, 15, null)]
    [InlineData("", 15, "")]
    [InlineData(" ", 15, " ")]
    [InlineData("ABC", 3, "ABC")]
    [InlineData("ALESFORD CAMPUS", 15, "ALESFORD CAMPUS")]
    [InlineData("ST MARY'S CATHOLIC COLLEGE, A VOLUNTARY ACADEMY", 15, "ST MARY'S CATHO...")]
    [InlineData("Warwick Green", 15, "Warwick Green")]
    // ReSharper restore StringLiteralTypo
    public void String_TruncateWithEllipsis_Data_Tests(string input, int length, string expectedResult)
    {
        var result = input.TruncateWithEllipsis(length);
        result.Should().Be(expectedResult);
    }
}