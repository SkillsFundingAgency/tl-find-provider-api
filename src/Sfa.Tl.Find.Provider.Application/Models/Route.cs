using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay(" {" + nameof(Id) + "}" +
                 " {" + nameof(Name) + "}" +
                 " ({" + nameof(NumberOfQualifications) + ", nq})")]
public class Route
{
    public int Id { get; init; }
    public string Name { get; init; }
    public int NumberOfQualifications => Qualifications?.Count ?? 0;
    public int NumberOfQualificationsOffered { get; set; }
    public IList<Qualification> Qualifications { get; init; } 
        = new List<Qualification>();
}