using Sfa.Tl.Find.Provider.Application.Models;
using FluentAssertions;

namespace Sfa.Tl.Find.Provider.Tests.Common.Extensions;
public static class GeoLocationValidationExtensions
{
    public static void Validate(this GeoLocation geoLocation,
        GeoLocation expectedGeoLocation)
    {
        geoLocation.Should().NotBeNull();
        geoLocation.Location.Should().Be(expectedGeoLocation.Location);
        geoLocation.Latitude.Should().Be(expectedGeoLocation.Latitude);
        geoLocation.Longitude.Should().Be(expectedGeoLocation.Longitude);
    }
}
