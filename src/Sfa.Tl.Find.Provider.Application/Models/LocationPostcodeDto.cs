using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay(" {" + nameof(LocationName) + "}" +
                 " {" + nameof(Postcode) + ", nq}")]
public class LocationPostcodeDto
{
    public int LocationId { get; init; }
    public string Postcode { get; init; }
    public string LocationName { get; init; }
}