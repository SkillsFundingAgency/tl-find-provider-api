using Sfa.Tl.Find.Provider.Application.Models.Enums;

namespace Sfa.Tl.Find.Provider.Application.Models;
public class NotificationLocationSummary
{
    public int? Id { get; init; }

    public NotificationFrequency Frequency { get; init; }

    public int? SearchRadius { get; init; }

    public LocationPostcode? Location { get; init; }

    public ICollection<Route> Routes { get; init; }
}
