using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay(" {" + nameof(Name) + "}" +
                 " {" + nameof(Postcode) + ", nq}")]
public class NamedLocation
{
    [JsonPropertyName("location")]
    public string Name { get; init; }

    public string Postcode { get; init; }
}