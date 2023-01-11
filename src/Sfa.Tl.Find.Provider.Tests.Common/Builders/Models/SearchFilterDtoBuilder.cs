using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class SearchFilterDtoBuilder
{
    public IEnumerable<SearchFilterDto> BuildList() =>
        new SearchFilterBuilder()
            .BuildList()
            .Select(s => new SearchFilterDto
            {
                SearchRadius = s.SearchRadius,
            })
            .ToList();
}