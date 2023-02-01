using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class SearchFilterBuilder
{
    private int? _searchRadius = 20;

    public IEnumerable<SearchFilter> BuildList() =>
        new List<SearchFilter>
        {
            new()
            {
                Id = 1,
                LocationId = 1,
                LocationName = "Test Location",
                Postcode = "CV1 2WT",
                SearchRadius = _searchRadius,
                Routes = new List<Route>
                {
                    new()
                    {
                        Id = 6,
                        Name = "Digital and IT"
                    }
                }
            },
            new()
            {
                Id = 2,
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

    public SearchFilter Build() =>
        BuildList()
            .First();

    public SearchFilterBuilder WithSearchRadius(int? searchRadius)
    {
        _searchRadius = searchRadius;
        return this;
    }
}