using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class EmployerInterestSummaryDtoBuilder
{
    public IEnumerable<EmployerInterestSummaryDto> BuildList() =>
        new List<EmployerInterestSummaryDto>
        {
            new()
            {
                Id = 1,
                OrganisationName = "Test Employer",
                Postcode = "CV1 2WT",
                Distance = 1.0,
                Industry = "",
                CreatedOn = DateTime.Parse("2022-10-01 12:00"),
                ModifiedOn = null
            },
            new()
            {
                Id = 2,
                OrganisationName = "Test Employer 2",
                Postcode = "CV2 2WT",
                Distance = 1.0,
                Industry = "",
                CreatedOn = DateTime.Parse("2022-10-01 12:00"),
                ModifiedOn = null
            }
        };

    public IList<EmployerInterestSummaryDto> BuildList(int numberToTake) =>
        BuildList().Take(numberToTake).ToList();

    public EmployerInterestSummaryDto Build() =>
        BuildList().First();
}