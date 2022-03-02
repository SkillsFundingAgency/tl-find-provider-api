namespace Sfa.Tl.Find.Provider.Api.Web.Services;

public class LocationSearchResult
{
    public string LocationName { get; init; }
    public string CountyName { get; init; }
    public string LocalAuthorityName { get; init; }
    public double Lat { get; init; }
    public double Long { get; init; }
}