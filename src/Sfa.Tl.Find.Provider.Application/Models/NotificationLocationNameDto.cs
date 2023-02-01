namespace Sfa.Tl.Find.Provider.Application.Models;
public class NotificationLocationNameDto
{
    public int? NotificationLocationId { get; init; }

    public int? LocationId { get; init; }
    
    public string LocationName { get; init; }

    public string Postcode { get; init; }
}
