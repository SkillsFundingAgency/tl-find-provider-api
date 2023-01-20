using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class NotificationDtoBuilder
{
    public IEnumerable<NotificationDto> BuildList() =>
        new NotificationBuilder()
            .BuildList()
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                LocationId = n.LocationId,
                LocationName = n.LocationName,
                Email = n.Email,
                Postcode = n.Postcode,
                SearchRadius = n.SearchRadius
            })
            .ToList();
}