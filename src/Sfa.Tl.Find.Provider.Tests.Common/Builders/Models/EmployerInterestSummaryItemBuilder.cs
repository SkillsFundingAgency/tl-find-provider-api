using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class EmployerInterestSummaryItemBuilder
{
    public IEnumerable<EmployerInterestSummaryItem> BuildList() =>
        new List<EmployerInterestSummaryItem>
        {
            new()
            {
                Id = 1,
                OrganisationName = "Test Employer",
                Distance = 1.0,
                Industry = "",
                CreatedOn = DateTime.Parse("2022-10-01 12:00"),
                ModifiedOn = null
            },
            new()
            {
                Id = 2,
                OrganisationName = "Test Employer 2",
                Distance = 1.0,
                Industry = "",
                CreatedOn = DateTime.Parse("2022-10-01 12:00"),
                ModifiedOn = null
            }
        };

    public IList<EmployerInterestSummaryItem> BuildList(int numberToTake) =>
        BuildList().Take(numberToTake).ToList();

    public EmployerInterestSummaryItem Build() =>
        BuildList().First();
}