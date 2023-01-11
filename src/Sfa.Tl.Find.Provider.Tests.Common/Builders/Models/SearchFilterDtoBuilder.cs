using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class SearchFilterDtoBuilder
{
    public IEnumerable<SearchFilterDto> BuildList() =>
        new SearchFilterBuilder()
            .BuildList()
            .Select(s => new SearchFilterDto
            {
                Id = s.Id,
                LocationId = s.LocationId,
                LocationName = s.LocationName,
                Postcode = s.Postcode,
                SearchRadius = s.SearchRadius,
            })
            .ToList();
}