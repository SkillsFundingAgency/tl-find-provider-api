using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("{DebuggerDisplay(), nq}")]
public class DeliveryYearDetail
{
    public short Year { get; init; }

    public bool IsAvailableNow { get; set; }

    public ICollection<RouteDetail> Routes { get; init; } = new List<RouteDetail>();

    private string DebuggerDisplay()
        => $"{Year} " +
           $"{(Routes != null ? Routes.Count : "null")} Routes";
}