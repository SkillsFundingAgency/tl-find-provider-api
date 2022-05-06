using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;

namespace Sfa.Tl.Find.Provider.Api.Models;

[DebuggerDisplay("{DebuggerDisplay(), nq}")]
public class ProviderSearchResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Error { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string SearchTerm { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TotalResults { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<ProviderSearchResult> SearchResults { get; init; }

    private string DebuggerDisplay()
        => $"{SearchTerm ?? "No search term"}, " +
           $"{(SearchResults != null ? SearchResults.Count() : "null")} SearchResults " +
           $"{Error ?? ""}";
}