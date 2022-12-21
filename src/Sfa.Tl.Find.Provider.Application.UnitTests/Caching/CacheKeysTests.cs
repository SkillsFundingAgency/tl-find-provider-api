using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Caching;

public class CacheKeysTests
{
    [Theory(DisplayName = nameof(CacheKeys.LatLongKey) + " Data Tests")]
    [InlineData(52.400997, -1.508122, "LAT_LONG__52.400997_-1.508122")]
    public void LatLong_Key_Returns_Expected_Value(double latitude, double longitude, string expectedKey)
    {
        var key = CacheKeys.LatLongKey(latitude, longitude);

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

    [Theory(DisplayName = nameof(CacheKeys.UserCacheKey) + " Data Tests")]
    [InlineData("7ff3469a-0c11-4c16-814b-2b9c5aaadf34", 
        CacheKeys.UserSessionActivityKey, 
        "USERID:7ff3469a-0c11-4c16-814b-2b9c5aaadf34:USER_SESSION_ACTIVITY")]
    public void User_Key_Returns_Expected_Value(string userId, string subKey, string expectedKey)
    {
        var key = CacheKeys.UserCacheKey(userId, subKey);
        key.Should().Be(expectedKey);
    }

    [Fact]
    public void PostcodeKey_Throws_Exception_For_Null_Postcode()
    {
        Action act = () => CacheKeys.PostcodeKey(null!);

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

    [Fact]
    public void GenerateTypedCacheKey_Returns_Expected_Value_For_String_Value()
    {
        const string key = "Test";
        const string expectedCacheKey = "test:string";

        var cacheKey = CacheKeys.GenerateTypedCacheKey<string>(key);
        cacheKey.Should().Be(expectedCacheKey);
    }

    [Fact]
    public void GenerateTypedCacheKey_Returns_Expected_Value_For_Generic_List_Value()
    {
        const string key = "Test";
        const string expectedCacheKey = "test:list<geolocation>";

        var cacheKey = CacheKeys.GenerateTypedCacheKey<List<GeoLocation>>(key);
        cacheKey.Should().Be(expectedCacheKey);
    }
}