using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("Id {" + nameof(Id) + "}" +
                 " {" + nameof(Email) + ", nq}")]
public class NotificationSummaryDto
{
    public int? Id { get; init; }

    public string Email { get; init; }

    public bool IsEmailVerified { get; init; }
}