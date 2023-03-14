using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("FileSize {" + nameof(FileSize) + ", nq}")]
public class ProviderDataDownloadInfoResponse
{
    public DateTime FileDate { get; init; }
    public string FormattedFileDate { get; init; }
    public int FileSize { get; init; }
}