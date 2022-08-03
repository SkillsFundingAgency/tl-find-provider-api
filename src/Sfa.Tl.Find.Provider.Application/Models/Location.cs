using System.Diagnostics;
using Dapper.Contrib.Extensions;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("{" + nameof(Postcode) + "}" +
                 " {" + nameof(Name) + ", nq}")]
public class Location
{
    public string Name { get; init; }
    public string Postcode { get; init; }
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

    [Write(false)]
    public ICollection<DeliveryYear> DeliveryYears { get; init; }
}