using System;
using System.Net;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Extensions;

public static class BusinessRuleExtensions
{
    public static bool IsAvailableAtDate(this short deliveryYear, DateTime today)
    {
        return deliveryYear < today.Year
               || (deliveryYear == today.Year && today.Month >= 9);
    }

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