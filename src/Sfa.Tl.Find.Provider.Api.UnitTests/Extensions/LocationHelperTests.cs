using FluentAssertions;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Extensions;

public class LocationTests
{
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
}