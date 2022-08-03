using System.Diagnostics;
// ReSharper disable All

namespace Sfa.Tl.Find.Provider.Api.Models;

[DebuggerDisplay("UKPRN {" + nameof(UkPrn) + "}" +
                 " {" + nameof(Postcode) + "}" +
                 " {" + nameof(Name) + ", nq}")]
public class LocationDto
{
    public long UkPrn { get; init; }
    public string Postcode { get; init; }
    public string Name { get; init; }
    public string AddressLine1 { get; init; }
    public string AddressLine2 { get; init; }
    public string Town { get; init; }
    public string County { get; init; }
    public string Email { get; init; }
    public string Telephone { get; init; }
    public string Website { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public bool IsAdditionalData { get; init; }
}