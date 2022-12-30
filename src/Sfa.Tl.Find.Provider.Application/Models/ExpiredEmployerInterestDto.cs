using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("UKPRN {" + nameof(Id) + "}" +
                 " {" + nameof(Email) + ", nq}")]
public class ExpiredEmployerInterestDto
{
    public int Id { get; init; }
    public Guid UniqueId { get; init; }
    public string Email { get; init; }
}
