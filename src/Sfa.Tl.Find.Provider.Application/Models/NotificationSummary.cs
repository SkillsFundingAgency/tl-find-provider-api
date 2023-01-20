using Sfa.Tl.Find.Provider.Application.Models.Enums;
using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("{DebuggerDisplay(), nq}")]
public class NotificationSummary
{
    public int? Id { get; init; }

    public string Email { get; init; }

    public bool IsEmailVerified { get; init; }

    public ICollection<LocationPostcode> Locations { get; init; }

    private string DebuggerDisplay()
        => $"Id {Id}, " +
           $"{Email} ({(IsEmailVerified ? "verified" : "pending")}), " +
           $"{(Locations != null ? Locations.Count : "null")} Locations";
}