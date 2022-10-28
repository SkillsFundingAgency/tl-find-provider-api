using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public static class GeoLocationBuilder
{
    public static GeoLocation BuildValidPostcodeLocation() =>
        new()
        {
            Location = "CV1 2WT",
            Latitude = 52.400997,
            Longitude = -1.508122
        };

    public static GeoLocation BuildValidOutcodeLocation() =>
        new()
        {
            Location = "CV1",
            Latitude = 52.4093942342931,
            Longitude = -1.50652551178011
        };

    public static GeoLocation BuildNotFoundPostcodeLocation() =>
        new()
        {
            Location = "CV1 9XT",
            Latitude = double.NaN,
            Longitude = double.NaN
        };

    public static GeoLocation BuildInvalidPostcodeLocation() =>
        new()
        {
            Location = "CV99 XXX",
            Latitude = double.NaN,
            Longitude = double.NaN
        };

    public static GeoLocation BuildInvalidOutcodeLocation() =>
        new()
        {
            Location = "L99",
            Latitude = double.NaN,
            Longitude = double.NaN
        };


    public static GeoLocation BuildTerminatedPostcodeLocation() =>
        new()
        {
            Location = "S70 2YW",
            Latitude = 53.551618,
            Longitude = -1.482797
        };

    public static GeoLocation BuildPostcodeLocationWithDefaultLatLong() =>
        new()
        {
            Location = "GY1 4AL",
            Latitude = Constants.DefaultLatitude,
            Longitude = Constants.DefaultLongitude
        };

    public static GeoLocation BuildOutcodeLocationWithDefaultLatLong() =>
        new()
        {
            Location = "IM4",
            Latitude = Constants.DefaultLatitude,
            Longitude = Constants.DefaultLongitude
        };

    public static GeoLocation BuildTerminatedPostcodeLocationWithDefaultLatLong() =>
        new()
        {
            Location = "IM4 4AQ",
            Latitude = Constants.DefaultLatitude,
            Longitude = Constants.DefaultLongitude
        };

    public static GeoLocation BuildGeoLocation(
        string location,
        double latitude = Constants.DefaultLatitude,
        double longitude = Constants.DefaultLongitude) =>
        new()
        {
            Location = location,
            Latitude = latitude,
            Longitude = longitude
        };
}