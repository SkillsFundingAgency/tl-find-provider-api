using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay(" {" + nameof(RouteId) + "}" +
                 " {" + nameof(RouteName) + ", nq})")]
public class RouteDto
{
    public int RouteId { get; init; }
    public string RouteName { get; init; }
}