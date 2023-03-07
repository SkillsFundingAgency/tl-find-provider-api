using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("{" + nameof(LocationPostcode) + "}" +
                 " {" + nameof(LocationName) + ", nq}")]
public class LocationDetailDto
{
    public string LocationName { get; init; }
    public string LocationPostcode { get; init; }
    public string LocationAddressLine1 { get; init; }
    public string LocationAddressLine2 { get; init; }
    public string LocationTown { get; init; }
    public string LocationCounty { get; init; }
    public string LocationEmail { get; init; }
    public string LocationTelephone { get; init; }
    public string LocationWebsite { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }  
}