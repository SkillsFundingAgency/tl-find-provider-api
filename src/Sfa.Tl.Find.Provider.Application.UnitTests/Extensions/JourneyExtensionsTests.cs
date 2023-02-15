using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Extensions;

public class JourneyExtensionsTests
{
    public static IEnumerable<object[]> JourneyLinksForPostcodeLocations =>
        new List<object[]>
        {
            new object[] { null, null, null },
            new object[] { new GeoLocation { Location = "" }, new GeoLocation { Location = "" }, null },
            new object[] { new GeoLocation { Location = "CV1 2WT", Latitude = 52.400997, Longitude = -1.508122 }, null, null },
            new object[] { null, new GeoLocation { Location = "SW1A 2HE", Latitude = 51.506041, Longitude = -0.123846 }, null },
            new object[]
            {
                new GeoLocation { Location = "CV1 2WT", Latitude = 52.400997, Longitude = -1.508122 },
                new GeoLocation { Location = "SW1A 2HE", Latitude = 51.506041, Longitude = -0.123846 },
                "https://www.google.com/maps/dir/?api=1&origin=CV1+2WT&destination=SW1A+2HE&travelmode=transit"
            },
            new object[]
            {
                new GeoLocation { Location = "SW1A 2HE", Latitude = 51.506041, Longitude = -0.123846 },
                new GeoLocation { Location = "CV1 2WT", Latitude = 52.400997, Longitude = -1.508122 },
                "https://www.google.com/maps/dir/?api=1&origin=SW1A+2HE&destination=CV1+2WT&travelmode=transit"
            }
        };

    [Theory(DisplayName = $"{nameof(JourneyExtensions.CreateJourneyLink)} Data Tests")]
    [MemberData(nameof(JourneyLinksForPostcodeLocations))]
    public void GeoLocation_CreateJourneyLink_Data_Tests(
        GeoLocation fromGeoLocation, 
        GeoLocation toGeoLocation, 
        string expectedResult)
    {
        var result = fromGeoLocation.CreateJourneyLink(toGeoLocation);
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = $"{nameof(JourneyExtensions.CreateJourneyLink)} Data Tests")]
    [MemberData(nameof(JourneyLinksForPostcodeLocations))]
    public void GeoLocation_CreateJourneyLink_To_Postcode_String_Data_Tests(
        GeoLocation fromGeoLocation,
        GeoLocation toGeoLocation,
        string expectedResult)
    {
        var result = fromGeoLocation.CreateJourneyLink(toGeoLocation?.Location);
        result.Should().Be(expectedResult);
    }
}