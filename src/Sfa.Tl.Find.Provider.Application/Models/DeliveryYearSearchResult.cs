using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("{DebuggerDisplay(), nq}")]
public class DeliveryYearSearchResult
{
    public short Year { get; init; }

    public bool IsAvailableNow { get; set; }

    public ICollection<Route> Routes { get; init; } = new List<Route>();

    private string DebuggerDisplay()
        => $"{Year} " +
           $"{(Routes != null ? Routes.Count : "null")} Routes";
}