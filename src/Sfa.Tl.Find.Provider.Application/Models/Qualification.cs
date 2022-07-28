using System.Diagnostics;
using Dapper.Contrib.Extensions;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay(" {" + nameof(Id) + "}" +
                 " {" + nameof(Name) + ", nq}")]
public class Qualification
{
    public int Id { get; init; }
    public string Name { get; init; }
    [Write(false)]
    public int NumberOfQualificationsOffered { get; init; }
}