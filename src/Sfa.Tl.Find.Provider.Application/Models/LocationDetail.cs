using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("{" + nameof(Postcode) + "}" +
                 " {" + nameof(Name) + ", nq}")]
public class LocationDetail
{
    [Column("LocationName")]
    public string Name { get; init; }
    public string Postcode { get; init; }
    [Column("LocationAddressLine1")]
    public string AddressLine1 { get; init; }
    [Column("LocationAddressLine2")]
    public string AddressLine2 { get; init; }
    public string Town { get; init; }
    public string County { get; init; }
    public string Email { get; init; }
    public string Telephone { get; init; }
    public string Website { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }

    public ICollection<DeliveryYearDetail> DeliveryYears { get; init; } = new List<DeliveryYearDetail>();
}