using System.Diagnostics;
using System.Text.Json.Serialization;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("{" + nameof(Postcode) + "}" +
                 " {" + nameof(Distance) + ", nq}")]
public class ProviderSearchResult
{
    [JsonIgnore]
    public long UkPrn { get; init; }
    public string ProviderName { get; init; }
    [JsonIgnore]
    public string LocationName { get; init; }
    public string Postcode { get; init; }
    [JsonIgnore]
    public string AddressLine1 { get; init; }
    [JsonIgnore]
    public string AddressLine2 { get; init; }
    public string Town { get; init; }
    [JsonIgnore]
    public string County { get; init; }
    public string Email { get; init; }
    public string Telephone { get; init; }
    public string Website { get; init; }
    public double Distance { get; init; }
    [JsonIgnore]
    public string JourneyToLink { get; set; }

    public ICollection<DeliveryYearSearchResult> DeliveryYears { get; init; } = new List<DeliveryYearSearchResult>();
}