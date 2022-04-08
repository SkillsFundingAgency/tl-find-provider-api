using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Api.Models;

[DebuggerDisplay(" {" + nameof(Location) + ", nq}" +
                 " ({" + nameof(Latitude) + "}, {" + nameof(Longitude) + "})")]
public class GeoLocation
{
    public string Location { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}