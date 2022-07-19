using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay(" {" + nameof(RouteId) + "}" +
                 " {" + nameof(RouteName) + ", nq})")]
public class RouteDetail
{
    public int RouteId { get; init; }
    public string RouteName { get; init; }
    public IList<QualificationDetail> Qualifications { get; init; } 
        = new List<QualificationDetail>();
}