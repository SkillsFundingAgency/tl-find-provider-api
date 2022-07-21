using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay(" {" + nameof(Id) + "}" +
                 " {" + nameof(Name) + ", nq}")]
public class QualificationDetail
{
    [Column("QualificationId")]
    public int Id { get; init; }
    [Column("QualificationName")]
    public string Name { get; init; }
}