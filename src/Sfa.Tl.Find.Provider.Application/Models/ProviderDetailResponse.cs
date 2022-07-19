using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("{DebuggerDisplay(), nq}")]
public class ProviderDetailResponse
{
    public IEnumerable<ProviderDetail> Providers { get; init; }

    private string DebuggerDisplay() =>
        $"{(Providers != null ? Providers.Count() : "null")} Providers ";
}