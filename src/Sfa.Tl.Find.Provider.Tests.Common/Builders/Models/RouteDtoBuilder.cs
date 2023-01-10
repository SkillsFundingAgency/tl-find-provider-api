using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class RouteDtoBuilder
{
    public IEnumerable<RouteDto> BuildList() =>
        new RouteBuilder()
            .BuildList()
            .Select(r => new RouteDto
            {
                RouteId = r.Id,
                RouteName = r.Name
            })
            .ToList();
}