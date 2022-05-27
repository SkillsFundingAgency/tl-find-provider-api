using System.Collections.Generic;
using System.Diagnostics;
using Dapper.Contrib.Extensions;

namespace Sfa.Tl.Find.Provider.Api.Models;

[DebuggerDisplay("{DebuggerDisplay(), nq}")]
public class DeliveryYearSearchResult
{
    public short Year { get; init; }

    [Write(false)]
    public bool IsAvailableNow { get; set; }

    public ICollection<Route> Routes { get; init; } = new List<Route>();

    private string DebuggerDisplay()
        => $"{Year} " +
           $"{(Routes != null ? Routes.Count : "null")} Routes";
}