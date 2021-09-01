using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sfa.Tl.Find.Provider.Api.Models
{
    [DebuggerDisplay("{DebuggerDisplay(), nq}")]
    public class ProviderSearchResponse
    {
        public string Postcode { get; init; }
        public IEnumerable<ProviderSearchResult> SearchResults { get; init; }

        private string DebuggerDisplay()
            => $"{Postcode} " +
               $"{(SearchResults != null ? SearchResults.Count() : "null")} SearchResults";
    }
}
