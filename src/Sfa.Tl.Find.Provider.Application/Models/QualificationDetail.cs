using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay(" {" + nameof(QualificationId) + "}" +
                 " {" + nameof(QualificationName) + ", nq}")]
public class QualificationDetail
{
    public int QualificationId { get; init; }
    public string QualificationName { get; init; }
}