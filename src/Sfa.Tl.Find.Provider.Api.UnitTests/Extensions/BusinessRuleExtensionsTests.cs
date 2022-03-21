using System;
using System.Collections.Generic;
using FluentAssertions;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Models;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Extensions;

public class BusinessRuleExtensionsTests
{
    [Theory(DisplayName = nameof(BusinessRuleExtensions.IsAvailableAtDate) + " Data Tests")]
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
        
    public static IEnumerable<object[]> JourneyLinksForPostcodeLocations =>
        new List<object[]>
        {
            new object[] { null, null, null },
            new object[] { new PostcodeLocation { Postcode = "" }, new PostcodeLocation { Postcode = "" }, null },
            new object[] { new PostcodeLocation { Postcode = "CV1 2WT", Latitude = 52.400997, Longitude = -1.508122 }, null, null },
            new object[] { null, new PostcodeLocation { Postcode = "SW1A 2HE", Latitude = 51.506041, Longitude = -0.123846 }, null },
            new object[]
            {
                new PostcodeLocation { Postcode = "CV1 2WT", Latitude = 52.400997, Longitude = -1.508122 },
                new PostcodeLocation { Postcode = "SW1A 2HE", Latitude = 51.506041, Longitude = -0.123846 },
                "https://www.google.com/maps/dir/?api=1&origin=CV1+2WT&destination=SW1A+2HE&travelmode=transit"
            },
            new object[]
            {
                new PostcodeLocation { Postcode = "SW1A 2HE", Latitude = 51.506041, Longitude = -0.123846 },
                new PostcodeLocation { Postcode = "CV1 2WT", Latitude = 52.400997, Longitude = -1.508122 },
                "https://www.google.com/maps/dir/?api=1&origin=SW1A+2HE&destination=CV1+2WT&travelmode=transit"
            }
        };

    [Theory(DisplayName = nameof(BusinessRuleExtensions.CreateJourneyLink) + " Data Tests")]
    [MemberData(nameof(JourneyLinksForPostcodeLocations))]
    public void PostcodeLocation_CreateJourneyLink_Data_Tests(
        PostcodeLocation fromPostcodeLocation, 
        PostcodeLocation toPostcodeLocation, 
        string expectedResult)
    {
        var result = fromPostcodeLocation.CreateJourneyLink(toPostcodeLocation);
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(BusinessRuleExtensions.CreateJourneyLink) + " Data Tests")]
    [MemberData(nameof(JourneyLinksForPostcodeLocations))]
    public void PostcodeLocation_CreateJourneyLink_To_Postcode_String_Data_Tests(
        PostcodeLocation fromPostcodeLocation,
        PostcodeLocation toPostcodeLocation,
        string expectedResult)
    {
        var result = fromPostcodeLocation.CreateJourneyLink(toPostcodeLocation?.Postcode);
        result.Should().Be(expectedResult);
    }
}