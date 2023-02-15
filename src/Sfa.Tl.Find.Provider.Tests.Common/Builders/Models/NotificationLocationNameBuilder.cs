using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class NotificationLocationNameBuilder
{
    private bool _withNullFirstLocation;

    public IEnumerable<NotificationLocationName> BuildList() =>
        new List<NotificationLocationName>
        {
            new()
            {
                Id = 1,
                LocationId = _withNullFirstLocation ? null : 1,
                Name = _withNullFirstLocation ? null : "Test Location 1",
                Postcode = _withNullFirstLocation ? null : "CV1 2WT"
            },
            new()
            {
                Id = 2,
                LocationId = 2,
                Name = "Test Location 2",
                Postcode = "CV2 3WT"
            },
            new()
            {
                Id = null,
                LocationId = 3,
                Name = "Test Location 3  (Available)",
                Postcode = "CV2 4WT"
            },
            new()
            {
                Id = null,
                LocationId = 4,
                Name = "Test Location 4 (Available)",
                Postcode = "CV2 5WT"
            }
        };

    public NotificationLocationName Build() =>
        BuildList()
            .First();

    public IEnumerable<NotificationLocationName> BuildListOfAvailableLocations() =>
        BuildList()
            .Where(x => x.Id is null);

    public NotificationLocationNameBuilder WithNullFirstLocation()
    {
        _withNullFirstLocation = true;
        return this;
    }
}