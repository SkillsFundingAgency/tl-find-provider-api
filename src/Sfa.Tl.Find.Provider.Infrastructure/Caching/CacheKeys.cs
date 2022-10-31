namespace Sfa.Tl.Find.Provider.Infrastructure.Caching;

public static class CacheKeys
{
    public const string IndustriesKey = "INDUSTRIES";
    public const string QualificationsKey = "QUALIFICATIONS";
    public const string RoutesKey = "ROUTES";
    public const string ProviderDataDownloadInfoKey = "PROVIDER_DATA_DOWNLOAD_INFO";
    public const string UserSessionActivityKey = "USER_SESSION_ACTIVITY";

    public static string PostcodeKey(string postcode)
    {
        if (postcode is null)
            throw new ArgumentNullException(nameof(postcode));

        if (string.IsNullOrWhiteSpace(postcode))
            throw new ArgumentException("A non-empty postcode is required", nameof(postcode));

        return $"POSTCODE__{postcode.Replace(" ", "").ToUpper()}";
    }

    public static string LatLongKey(double latitude, double longitude) =>
        $"LAT_LONG__{latitude}_{longitude}";

    public static string UserCacheKey(string userId, string key) => 
        $"USERID:{userId}:{key}";
}