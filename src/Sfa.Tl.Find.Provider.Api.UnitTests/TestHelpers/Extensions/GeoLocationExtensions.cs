using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;

public static class GeoLocationExtensions
{
    public static string GetUriFormattedPostcode(this GeoLocation geoLocation)
    {
        return geoLocation.Location is not null ? 
            geoLocation.Location.Replace(" ", "%20")
            : string.Empty;
    }
}