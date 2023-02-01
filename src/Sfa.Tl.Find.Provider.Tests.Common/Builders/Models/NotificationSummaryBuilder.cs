using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class NotificationSummaryBuilder
{
    public IEnumerable<NotificationSummary> BuildList() =>
        new List<NotificationSummary>
        {
            new()
            {
                Id = 1,
                Email = "test@provider1.co.uk",
                IsEmailVerified = true,
                Locations = new List<NotificationLocationName>
                {
                    new()
                    {
                        Id = 1,
                        Name = "The Palace",
                        Postcode = "SW1A 1AA"
                    }
                }
            },
            new ()
            {
                Id = 2,
                Email = "test@provider2.co.uk",
                Locations = new List<NotificationLocationName>
                {
                    new()
                    {
                        Id = 1,
                        Name = "The Palace",
                        Postcode = "SW1A 1AA"
                    },
                    new()
                    {
                        Id = 2,
                        Name = "Headquarters",
                        Postcode = "SW1A 2AA"
                    }
                }
            }
        };

    public NotificationSummary Build() =>
        BuildList()
            .First();
}