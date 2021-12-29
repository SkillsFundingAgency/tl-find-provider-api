using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders;

public static class PostcodeLocationBuilder
{
    public static PostcodeLocation BuildValidPostcodeLocation() =>
        new()
        {
            Postcode = "CV1 2WT",
            Latitude = 52.400997,
            Longitude = -1.508122
        };

    public static PostcodeLocation BuildValidOutwardPostcodeLocation() =>
        new()
        {
            Postcode = "CV1",
            Latitude = 52.4093942342931,
            Longitude = -1.50652551178011,
        };

    public static PostcodeLocation BuildNotFoundPostcodeLocation() =>
        new()
        {
            Postcode = "CV1 9XT",
            Latitude = double.NaN,
            Longitude = double.NaN
        };

    public static PostcodeLocation BuildInvalidPostcodeLocation() =>
        new()
        {
            Postcode = "CV99 XXX",
            Latitude = double.NaN,
            Longitude = double.NaN
        };

    public static PostcodeLocation BuildTerminatedPostcodeLocation() =>
        new()
        {
            Postcode = "S70 2YW",
            Latitude = 53.551618,
            Longitude = -1.482797
        };

    public static PostcodeLocation BuildPostcodeLocationWithDefaultLatLong() =>
        new()
        {
            Postcode = "GY1 4AL",
            Latitude = Constants.DefaultLatitude,
            Longitude = Constants.DefaultLongitude
        };

    public static PostcodeLocation BuildOutwardPostcodeLocationWithDefaultLatLong() =>
        new()
        {
            Postcode = "IM4",
            Latitude = Constants.DefaultLatitude,
            Longitude = Constants.DefaultLongitude,
        };

    public static PostcodeLocation BuildTerminatedPostcodeLocationWithDefaultLatLong() =>
        new()
        {
            Postcode = "IM4 4AQ",
            Latitude = Constants.DefaultLatitude,
            Longitude = Constants.DefaultLongitude,
        };

    public static PostcodeLocation BuildPostcodeLocation(
        string postcode) =>
        new()
        {
            Postcode = postcode,
            Latitude = 50.0,
            Longitude = -1.0
        };
}