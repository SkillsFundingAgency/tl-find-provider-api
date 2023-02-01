using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class NotificationLocationSummaryBuilder
{
    public IEnumerable<NotificationLocationSummary> BuildList() =>
        new List<NotificationLocationSummary>
        {
            new()
            {
                Id = 1,
                SearchRadius = 20,
                Frequency = NotificationFrequency.Daily,
                Location = new NotificationLocationName
                {
                    Id = 1,
                    Name = "The Palace",
                    Postcode = "SW1A 1AA"
                },
                Routes = new List<Route>
                {
                    new()
                    {
                        Id = 1,
                        Name = "Agriculture, environment and animal care"
                    },
                    new()
                    {
                        Id = 2,
                        Name = "Business and administration"
                    }
                }
            },
            new ()
            {
                Id = 2,
                SearchRadius = 20,
                Frequency = NotificationFrequency.Weekly,
                Location = null,
                Routes = new List<Route>()
            }
        };

    public NotificationLocationSummary Build() =>
        BuildList()
            .First();
}