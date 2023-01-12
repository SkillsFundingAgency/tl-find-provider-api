using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class NotificationBuilder
{
    public IEnumerable<Notification> BuildList() =>
        new List<Notification>
        {
            new()
            {
                Id = 1,
                LocationId = 1,
                LocationName= "Test Location 1",
                Postcode = "CV1 2WT",
                Email = "test@provider1.co.uk",
                SearchRadius = 20,
            },
            new()
            {
                Id = 2,
                LocationId = 1,
                LocationName= "Test Location 2",
                Postcode = "CV1 2WT",
                Email = "test@provider2.co.uk",
                SearchRadius = 25,
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
            }
        };

    public Notification Build() =>
        BuildList()
            .First();
}