using System;

namespace Sfa.Tl.Find.Provider.Api.Models;

public static class CacheKeys
{
    public const string QualificationsKey = "QUALIFICATIONS";

    public const string RoutesKey = "ROUTES";

    public static string PostcodeKey(string postcode)
    {
        if (postcode is null) 
            throw new ArgumentNullException(nameof(postcode));
            
        if (string.IsNullOrWhiteSpace(postcode)) 
            throw new ArgumentException("A non-empty postcode is required", nameof(postcode));

        return $"POSTCODE__{postcode.Replace(" ", "").ToUpper()}";
    }
}