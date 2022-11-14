using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class QualificationDtoBuilder
{
    public IEnumerable<QualificationDto> BuildList()
    {
        return new QualificationBuilder()
            .BuildList()
            .Select((q, idx) => new QualificationDto
            {
                QualificationId = q.Id,
                QualificationName = q.Name,
                NumberOfQualificationsOffered = idx + 1
            })
            .ToList();
    }
}