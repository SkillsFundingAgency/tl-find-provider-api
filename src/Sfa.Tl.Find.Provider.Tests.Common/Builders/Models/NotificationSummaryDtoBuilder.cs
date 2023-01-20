using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class NotificationSummaryDtoBuilder
{
    public IEnumerable<NotificationSummaryDto> BuildList() =>
        new NotificationSummaryBuilder()
            .BuildList()
            .Select(n => new NotificationSummaryDto
            {
                Id = n.Id,
                Email = n.Email,
                IsEmailVerified = n.IsEmailVerified
            })
            .ToList();
}