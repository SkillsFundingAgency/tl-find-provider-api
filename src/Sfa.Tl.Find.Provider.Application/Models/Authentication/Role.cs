using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models.Authentication;

[DebuggerDisplay("{" + nameof(Name) + ", nq}")]
public class Role
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Code { get; set; }
}