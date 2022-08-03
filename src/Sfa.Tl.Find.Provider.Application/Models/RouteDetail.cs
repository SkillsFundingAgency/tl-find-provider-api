using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay(" {" + nameof(RouteId) + "}" +
                 " {" + nameof(RouteName) + ", nq})")]
public class RouteDetail
{
    [JsonPropertyName("id")]
    public int RouteId { get; init; }
    [JsonPropertyName("name")]
    public string RouteName { get; init; }
    public IList<QualificationDetail> Qualifications { get; init; } 
        = new List<QualificationDetail>();
}