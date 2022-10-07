namespace Sfa.Tl.Find.Provider.Application.Models;

public static class CacheKeys
{
    public const string IndustriesKey = "INDUSTRIES";
    public const string QualificationsKey = "QUALIFICATIONS";
    public const string RoutesKey = "ROUTES";
    public const string ProviderDataDownloadInfoKey = "PROVIDER_DATA_DOWNLOAD_INFO";

    public static string PostcodeKey(string postcode)
    {
        if (postcode is null)
            throw new ArgumentNullException(nameof(postcode));

        if (string.IsNullOrWhiteSpace(postcode))
            throw new ArgumentException("A non-empty postcode is required", nameof(postcode));

        return $"POSTCODE__{postcode.Replace(" ", "").ToUpper()}";
    }

    public static string LatLongKey(double latitude, double longitude)
    {
        return $"LAT_LONG__{latitude}_{longitude}";
    }
}