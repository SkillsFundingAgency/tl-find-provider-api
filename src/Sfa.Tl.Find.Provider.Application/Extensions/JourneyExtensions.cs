using System.Net;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Extensions;

public static class JourneyExtensions
{
    public static string CreateJourneyLink(this GeoLocation from, string toPostcode)
    {
        return from.CreateJourneyLink(new GeoLocation { Location = toPostcode });
    }

    public static string CreateJourneyLink(this GeoLocation from, GeoLocation to)
    {
        if (string.IsNullOrEmpty(from?.Location) ||
            string.IsNullOrEmpty(to?.Location))
            return null;

        return "https://www.google.com/maps/dir/?api=1&" +
               $"origin={WebUtility.UrlEncode(from.Location.Trim())}" +
               $"&destination={WebUtility.UrlEncode(to.Location.Trim())}" +
               "&travelmode=transit";
    }
}