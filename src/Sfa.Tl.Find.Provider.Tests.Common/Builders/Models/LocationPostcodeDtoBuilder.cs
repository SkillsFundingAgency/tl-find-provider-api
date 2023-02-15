using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class LocationPostcodeDtoBuilder
{
    public IEnumerable<LocationPostcodeDto> BuildList() =>
        new LocationPostcodeBuilder()
            .BuildList()
            .Select(l => new LocationPostcodeDto
            {
                LocationId = l.Id ?? 0,
                LocationName = l.Name,
                Postcode = l.Postcode
            })
            .ToList();
}