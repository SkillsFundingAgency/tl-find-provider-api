using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Sfa.Tl.Find.Provider.Api.Models;

[DebuggerDisplay(" {" + nameof(Name) + "}" +
                 " ({" + nameof(County) + ", nq})")]
public class Town
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string County { get; init; }
    [JsonIgnore]
    public string LocalAuthorityName { get; init; }
    [JsonIgnore]
    public decimal Latitude { get; init; }
    [JsonIgnore]
    public decimal Longitude { get; init; }
}