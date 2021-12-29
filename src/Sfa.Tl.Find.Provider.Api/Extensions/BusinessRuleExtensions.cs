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

    public static string CreateJourneyLink(this PostcodeLocation from, string toPostcode)
    {
        return from.CreateJourneyLink(new PostcodeLocation { Postcode = toPostcode });
    }

    public static string CreateJourneyLink(this PostcodeLocation from, PostcodeLocation to)
    {
        if (string.IsNullOrEmpty(from?.Postcode) ||
            string.IsNullOrEmpty(to?.Postcode))
            return null;

        return "https://www.google.com/maps/dir/?api=1&" +
               $"origin={WebUtility.UrlEncode(from.Postcode.Trim())}" +
               $"&destination={WebUtility.UrlEncode(to.Postcode.Trim())}" +
               "&travelmode=transit";
    }
}