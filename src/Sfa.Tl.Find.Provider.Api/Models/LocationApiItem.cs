using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Api.Models;

[DebuggerDisplay(" {" + nameof(Name) + "}" +
                 " ({" + nameof(County) + ", nq})")]
public class LocationApiItem
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string County { get; init; }
    public string LocalAuthorityName { get; init; }
    public string LocalAuthorityDistrict { get; init; }
    public string LocalAuthorityDistrictDescription { get; init; }
    public string PlaceNameDescription { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
}