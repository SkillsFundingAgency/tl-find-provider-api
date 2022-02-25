using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Api.Models;

[DebuggerDisplay(" {" + nameof(Id) + "}" +
                 " UKPRN {" + nameof(UkPrn) + "}" +
                 " URN {" + nameof(Urn) + "}" +
                 " {" + nameof(Name) + ", nq}")]
public class ProviderReference
{
    public int Id { get; init; }
    public long UkPrn { get; init; }
    public long Urn { get; init; }
    public string Name { get; init; }
}