using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay(" {" + nameof(QualificationId) + "}" +
                 " {" + nameof(QualificationName) + "}" +
                 " ({" + nameof(NumberOfQualificationsOffered) + ", nq})")]
public class QualificationDto
{
    public int QualificationId { get; init; }
    public string QualificationName { get; init; }
    public int NumberOfQualificationsOffered { get; init; }
}