using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class EmployerInterestSummaryBuilder
{
    private DateTime _creationDate = DateTime.Parse("2022-10-01 12:00");
    private DateTime? _modificationDate = null;

    public IEnumerable<EmployerInterestSummary> BuildList() =>
        new List<EmployerInterestSummary>
        {
            new()
            {
                Id = 1,
                OrganisationName = "Test Employer",
                Distance = 1.0,
                Industry = "",
                SkillAreas = new List<string>
                {
                    "Digital and IT"
                },
                CreatedOn = _creationDate,
                ModifiedOn = _modificationDate
            },
            new()
            {
                Id = 2,
                OrganisationName = "Test Employer 2",
                Distance = 1.0,
                Industry = "",
                SkillAreas = new List<string>
                {
                    "Creative and design",
                    "Digital and IT"
                },
                CreatedOn = _creationDate,
                ModifiedOn = _modificationDate
            }
        };

    public IList<EmployerInterestSummary> BuildList(int numberToTake) =>
        BuildList().Take(numberToTake).ToList();

    public EmployerInterestSummary Build() =>
        BuildList().First();

    public EmployerInterestSummaryBuilder WithCreationDate(DateTime dateTime)
    {
        _creationDate = dateTime;
        return this;
    }

    public EmployerInterestSummaryBuilder WithModificationDate(DateTime dateTime)
    {
        _modificationDate = dateTime;
        return this;
    }

}