using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class NotificationDtoBuilder
{
    public IEnumerable<NotificationDto> BuildList() =>
        new NotificationBuilder()
            .BuildList()
            .Select(s => new NotificationDto
            {
                Id = s.Id,
                LocationId = s.LocationId,
                LocationName = s.LocationName,
                Email = s.Email,
                Postcode = s.Postcode,
                SearchRadius = s.SearchRadius
            })
            .ToList();
}