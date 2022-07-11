using System;
using FluentAssertions;
using Sfa.Tl.Find.Provider.Application.Models;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Caching;

public class CacheKeysTests
{
    [Theory(DisplayName = nameof(CacheKeys.LatLongKey) + " Data Tests")]
    [InlineData(52.400997, -1.508122, "LAT_LONG__52.400997_-1.508122")]
    public void LatLong_Key_Returns_Expected_Value(double latitude, double longitude, string expectedKey)
    {
        var key = CacheKeys.LatLongKey(latitude, longitude);
        // Expected key to be
        // "LAT_LONG__52.400997_-1.508122" with a length of 29,
        // "LAT_LONG__52.400997_-1.508122_)" has a length of 31, differs near "_)"(index 29).

        key.Should().Be(expectedKey);
    }

    [Theory(DisplayName = nameof(CacheKeys.PostcodeKey) + " Data Tests")]
    [InlineData("cv12wt", "POSTCODE__CV12WT")]
    [InlineData("CV1 2WT", "POSTCODE__CV12WT")]
    public void Postcode_Key_Returns_Expected_Value(string postcode, string expectedKey)
    {
        var key = CacheKeys.PostcodeKey(postcode);
        key.Should().Be(expectedKey);
    }

    [Fact]
    public void PostcodeKey_Throws_Exception_For_Null_Postcode()
    {
        Action act = () => CacheKeys.PostcodeKey(null);

        act.Should().Throw<ArgumentNullException>();

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("postcode");
    }

    [Fact]
    public void PostcodeKey_Throws_Exception_For_Empty_Postcode()
    {
        Action act = () => CacheKeys.PostcodeKey("");

        act.Should().Throw<ArgumentException>();

        act.Should().Throw<ArgumentException>()
            .WithMessage("A non-empty postcode is required*")
            .WithParameterName("postcode");
    }
}