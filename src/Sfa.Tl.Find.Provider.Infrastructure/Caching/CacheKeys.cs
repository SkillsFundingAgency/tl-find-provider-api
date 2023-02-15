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

    public static string GenerateTypedCacheKey<T>(string key)
    {
        return GenerateTypedCacheKey(typeof(T), key);
    }

    private static string GenerateTypedCacheKey(Type objectType, string key)
    {
        var typeName = ExpandTypeName(objectType);
        return $"{key}:{typeName}".ToLower();
    }

    private static string ExpandTypeName(Type t) =>
        !t.IsGenericType || t.IsGenericTypeDefinition
            ? !t.IsGenericTypeDefinition ? t.Name : t.Name.Remove(t.Name.IndexOf('`'))
            : $"{ExpandTypeName(t.GetGenericTypeDefinition())}<{string.Join(',', t.GetGenericArguments().Select(ExpandTypeName))}>";

}