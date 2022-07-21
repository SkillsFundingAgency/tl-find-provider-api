using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay(" {" + nameof(Id) + "}" +
                 " {" + nameof(Name) + ", nq})")]
public class RouteDetail
{
    [Column("RouteId")]
    public int Id { get; init; }
    [Column("RouteName")]
    public string Name { get; init; }
    public IList<QualificationDetail> Qualifications { get; init; } 
        = new List<QualificationDetail>();
}