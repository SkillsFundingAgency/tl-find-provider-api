using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("Id {" + nameof(Id) + "}" +
                 " {" + nameof(Email) + ", nq}")]
public class NotificationDto
{
    public int? Id { get; init; }

    public int LocationId { get; init; }

    public string LocationName { get; init; }

    public string Email { get; init; }

    public string Postcode { get; init; }

    public int? SearchRadius { get; init; }
}