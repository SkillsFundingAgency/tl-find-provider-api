using System.Diagnostics;
using Dapper.Contrib.Extensions;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("{DebuggerDisplay(), nq}")]
public class DeliveryYearSearchResult
{
    public short Year { get; init; }

    [Write(false)]
    public bool IsAvailableNow { get; set; }

    [Obsolete("Replaced with Routes in API v3")]
    public ICollection<Qualification> Qualifications { get; set; }

    //TODO: Change to init
    public ICollection<Route> Routes { get; set; } = new List<Route>();

    private string DebuggerDisplay()
        => $"{Year} " +
           $"{(Routes != null ? Routes.Count : "null")} Routes";
}