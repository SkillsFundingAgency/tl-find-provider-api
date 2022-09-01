using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("UKPRN {" + nameof(UkPrn) + "}" +
                 " {" + nameof(ProviderName) + ", nq}")]
public class ProviderDetailFlat
{
    public long UkPrn { get; init; }
    public string ProviderName { get; init; }
    public string Postcode { get; init; }
    public string LocationName { get; init; }
    public string AddressLine1 { get; init; }
    public string AddressLine2 { get; init; }
    public string Town { get; init; }
    public string County { get; init; }
    public string Email { get; init; }
    public string Telephone { get; init; }
    public string Website { get; init; }
    public short Year { get; init; }
    public int RouteId { get; init; }
    public string RouteName { get; init; }
    public int QualificationId { get; init; }
    public string QualificationName { get; init; }
}