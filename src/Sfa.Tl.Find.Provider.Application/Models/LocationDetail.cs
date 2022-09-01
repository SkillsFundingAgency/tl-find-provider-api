using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("{" + nameof(Postcode) + "}" +
                 " {" + nameof(LocationName) + ", nq}")]
public class LocationDetail
{
    [JsonPropertyName("locationName")]
    public string LocationName { get; init; }
    public string Postcode { get; init; }
    [JsonPropertyName("addressLine1")]
    public string LocationAddressLine1 { get; init; }
    [JsonPropertyName("addressLine2")]
    public string LocationAddressLine2 { get; init; }
    public string Town { get; init; }
    public string County { get; init; }
    public string Email { get; init; }
    public string Telephone { get; init; }
    public string Website { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }

    public ICollection<DeliveryYearDetail> DeliveryYears { get; init; } = new List<DeliveryYearDetail>();
}