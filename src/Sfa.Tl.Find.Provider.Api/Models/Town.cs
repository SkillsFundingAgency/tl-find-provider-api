using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Api.Models;

[DebuggerDisplay(" {" + nameof(Name) + "}" +
                 " ({" + nameof(County) + ", nq})")]
public class Town
{
    public string Name { get; init; }
    public string County { get; init; }
    public string LocalAuthorityName { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude  { get; init; }
}