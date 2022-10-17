using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class EmployerInterestSummaryBuilder
{
    public IEnumerable<EmployerInterestSummary> BuildList() =>
        new List<EmployerInterestSummary>
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

    public IList<EmployerInterestSummary> BuildList(int numberToTake) =>
        BuildList().Take(numberToTake).ToList();

    public EmployerInterestSummary Build() =>
        BuildList().First();
}