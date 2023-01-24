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
                Email = n.Email,
                IsEmailVerified = n.IsEmailVerified,
                EmailVerificationToken = n.EmailVerificationToken,
                LocationId = n.LocationId,
                LocationName = n.LocationName,
                Postcode = n.Postcode,
                SearchRadius = n.SearchRadius
            })
            .ToList();
}