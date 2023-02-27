using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay(" {" + nameof(Name) + "}" +
                 " {" + nameof(Postcode) + ", nq}")]
public class NamedLocation
{
    public string Name { get; init; }

    public string Postcode { get; init; }
}