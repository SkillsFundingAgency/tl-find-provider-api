using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay(" {" + nameof(Postcode) + ", nq}" +
                 " ({" + nameof(Latitude) + "}, {" + nameof(Longitude) + "})")]
public class LocationPostcode
{
    public int? Id { get; init; }
    public string Postcode { get; init; }
    public string Name { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}