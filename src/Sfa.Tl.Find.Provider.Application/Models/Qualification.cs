using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay(" {" + nameof(Id) + "}" +
                 " {" + nameof(Name) + ", nq}")]
public class Qualification
{
    public int Id { get; init; }
    public string Name { get; init; }
    public int NumberOfQualificationsOffered { get; set; }
}