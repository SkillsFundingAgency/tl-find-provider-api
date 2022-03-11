using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Api.Models;

[DebuggerDisplay(" {" + nameof(Id) + "}" +
                 " {" + nameof(Name) + "}" +
                 " ({" + nameof(County) + ", nq})")]
public class Town
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string County { get; init; }
    public string LocalAuthority { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude  { get; init; }
}