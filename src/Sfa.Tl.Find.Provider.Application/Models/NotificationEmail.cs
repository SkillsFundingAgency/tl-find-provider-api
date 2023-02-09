using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("Id {" + nameof(NotificationLocationId) + "}" +
                 " {" + nameof(Email) + ", nq}")]
public class NotificationEmail
{
    public int NotificationLocationId { get; init; }

    public string Email { get; init; }
}
