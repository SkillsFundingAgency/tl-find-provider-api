using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class NotificationEmailBuilder
{
    public IEnumerable<NotificationEmail> BuildList() =>
        new List<NotificationEmail>
        {
            new()
            {
                NotificationLocationId = 1,
                Email = "test1@test.com"
            },
            new()
            {
                NotificationLocationId = 2,
                Email = "test2@test.com"
            }
        };

    public NotificationEmail Build() =>
        BuildList()
            .First();
}