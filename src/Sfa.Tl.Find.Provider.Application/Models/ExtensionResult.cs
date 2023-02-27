using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("Success {" + nameof(Success) + "}," +
                 " Remaining {" + nameof(ExtensionsRemaining) + ", nq}")]
public class ExtensionResult
{
    public bool Success { get; init; } 
    public int ExtensionsRemaining { get; init; }
}
