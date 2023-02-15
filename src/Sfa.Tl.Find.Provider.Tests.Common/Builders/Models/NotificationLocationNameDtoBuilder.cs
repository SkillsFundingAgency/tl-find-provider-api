using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class NotificationLocationNameDtoBuilder
{
    public IEnumerable<NotificationLocationNameDto> BuildList() =>
        new NotificationLocationNameBuilder()
            .BuildList()
            .Select(l => new NotificationLocationNameDto
            {
                NotificationLocationId = l.Id,
                LocationId = l.Id,
                LocationName = l.Name,
                Postcode = l.Postcode
            })
            .ToList();
}