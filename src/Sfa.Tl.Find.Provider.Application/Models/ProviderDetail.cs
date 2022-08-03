using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("UKPRN {" + nameof(UkPrn) + "}" +
                 " {" + nameof(Name) + ", nq}")]
public class ProviderDetail
{
    public long UkPrn { get; init; }
    public string Name { get; init; }
    public string AddressLine1 { get; init; }
    public string AddressLine2 { get; init; }
    public string Town { get; init; }
    public string County  { get; init; }
    public string Postcode  { get; init; }
    public string Email { get; init; }
    public string Telephone { get; init; }
    public string Website { get; init; }
    public bool IsAdditionalData { get; init; }

    public ICollection<LocationDetail> Locations { get; init; } = new List<LocationDetail>();
}