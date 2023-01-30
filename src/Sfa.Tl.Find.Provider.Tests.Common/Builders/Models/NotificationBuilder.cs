using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class NotificationBuilder
{
    private bool _useNullId;
    private bool _useNullLocation;
    private int? _searchRadius = 20;
    private NotificationFrequency _frequency = NotificationFrequency.Immediately;
        

    public IEnumerable<Notification> BuildList() =>
        new List<Notification>
        {
            new()
            {
                Id = _useNullId ? null : 1,
                Email = "test@provider1.co.uk",
                IsEmailVerified = true,
                LocationId = _useNullLocation ? null : 1,
                LocationName= _useNullLocation ? null : "Test Location 1",
                Postcode = _useNullLocation ? null : "CV1 2WT",
                SearchRadius = _searchRadius,
                Frequency = _frequency
            },
            new()
            {
                Id = _useNullId ? null : 2,
                Email = "test@provider2.co.uk",
                IsEmailVerified = false,
                EmailVerificationToken = Guid.Parse("b61cd465-1836-4a44-bf60-51a1f7254285"),
                LocationId = _useNullLocation ? null : 2,
                LocationName= _useNullLocation ? null : "Test Location 2",
                Postcode = _useNullLocation ? null : "CV2 3WT",
                SearchRadius = _searchRadius,
                Frequency = _frequency,
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

    public NotificationBuilder WithSearchRadius(int? searchRadius)
    {
        _searchRadius = searchRadius;
        return this;
    }

    public NotificationBuilder WithFrequency(NotificationFrequency frequency)
    {
        _frequency = frequency;
        return this;
    }

    public NotificationBuilder WithNullId()
    {
        _useNullId = true;
        return this;
    }

    public NotificationBuilder WithNullLocation()
    {
        _useNullLocation = true;
        return this;
    }
}