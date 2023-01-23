﻿using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class NotificationBuilder
{
    private bool _useNullId;
    private int? _searchRadius = 20;
    private NotificationFrequency _frequency = NotificationFrequency.Immediately;
        

    public IEnumerable<Notification> BuildList() =>
        new List<Notification>
        {
            new()
            {
                Id = _useNullId ? null : 1,
                LocationId = 1,
                LocationName= "Test Location 1",
                Postcode = "CV1 2WT",
                Email = "test@provider1.co.uk",
                SearchRadius = _searchRadius,
                Frequency = _frequency
            },
            new()
            {
                Id = _useNullId ? null : 2,
                LocationId = 1,
                LocationName= "Test Location 2",
                Postcode = "CV1 2WT",
                Email = "test@provider2.co.uk",
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

    public NotificationBuilder WithNullId(bool useNullId = true)
    {
        _useNullId = useNullId;
        return this;
    }
}