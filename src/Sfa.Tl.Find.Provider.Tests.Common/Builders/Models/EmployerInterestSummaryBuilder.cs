using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class EmployerInterestSummaryBuilder
{
    private DateTime _creationDate = DateTime.Parse("2022-10-01 12:00");
    private DateTime? _expiryDate;
    private DateTime? _modificationDate;

    public IEnumerable<EmployerInterestSummary> BuildList() =>
        new List<EmployerInterestSummary>
        {
            new()
            {
                Id = 1,
                OrganisationName = "Test Employer",
                Postcode = "CV1 2WT",
                Distance = 1.0,
                Industry = "",
                SkillAreas = new List<string>
                {
                    "Digital and IT"
                },
                ExpiryDate = _expiryDate,
                CreatedOn = _creationDate,
                ModifiedOn = _modificationDate
            },
            new()
            {
                Id = 2,
                OrganisationName = "Test Employer 2",
                Postcode = "CV2 2WT",
                Distance = 1.0,
                Industry = "",
                SkillAreas = new List<string>
                {
                    "Creative and design",
                    "Digital and IT"
                },
                ExpiryDate = _creationDate,
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

    public EmployerInterestSummaryBuilder WithExpiryDate(DateTime dateTime)
    {
        _expiryDate = dateTime;
        return this;
    }
    public EmployerInterestSummaryBuilder WithModificationDate(DateTime dateTime)
    {
        _modificationDate = dateTime;
        return this;
    }
}