using Sfa.Tl.Find.Provider.Application.Models.Enums;
using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("{DebuggerDisplay(), nq}")]
public class Notification
{
    public int? Id { get; init; }

    public string Email { get; init; }

    public bool IsEmailVerified { get; init; }

    public Guid? EmailVerificationToken { get; set; }

    public NotificationFrequency Frequency { get; init; }

    public int? SearchRadius { get; init; }

    public int? LocationId { get; init; }

    public string LocationName { get; init; }

    public string Postcode { get; init; }

    public ICollection<Route> Routes { get; init; } = new List<Route>();

    private string DebuggerDisplay()
        => $"Id {Id}, " +
           $"{Email}, " +
           $"SearchRadius {SearchRadius}, " +
           $"{(Routes != null ? Routes.Count : "null")} Routes";
}