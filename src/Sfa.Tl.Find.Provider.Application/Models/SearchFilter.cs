using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("{DebuggerDisplay(), nq}")]
public class SearchFilter
{
    public int? Id { get; init; }

    public int LocationId { get; init; }

    public string LocationName { get; init; }

    public string Postcode { get; init; }

    public int? SearchRadius { get; init; }

    public ICollection<Route> Routes { get; init; } = new List<Route>();

    private string DebuggerDisplay()
        => $"Id {Id}, " +
           $"{LocationName}, " +
           $"SearchRadius {SearchRadius}, " +
           $"{(Routes != null ? Routes.Count : "null")} Routes";
}