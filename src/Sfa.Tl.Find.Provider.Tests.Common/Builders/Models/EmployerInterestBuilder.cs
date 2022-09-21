using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class EmployerInterestBuilder
{
    public IEnumerable<EmployerInterest> BuildList() =>
        new List<EmployerInterest>
        {
            new()
            {
                Id = 1,
                UniqueId = "847ad919-2c1e-45d2-a7da-080b325094a1",
                OrganisationName = "Test Employer",
                ContactName = "Test Contact",
                Postcode = "CV1 2WT",
                HasMultipleLocations = false,
                LocationCount = 1,
                IndustryId = 37,
                SpecificRequirements = "These are my requirements: none",
                Email ="test.contact1@emplyer.co.uk",
                Telephone ="020 555 6666 ext 1",
                ContactPreferenceType = 1,
                CreatedOn = DateTime.Parse("2022-10-01 09:00"),
                ModifiedOn = null
            },
            new()
            {
                Id = 2,
                UniqueId = "8e646160-c9eb-4700-a037-7ca613fd4099",
                OrganisationName = "Test Employer 2",
                ContactName = "Test Contact 2",
                Postcode = "CV1 3XT",
                HasMultipleLocations = false,
                LocationCount = 1,
                IndustryId = 51,
                SpecificRequirements = "These are my requirements: a few good people",
                Email ="test.contact2@emplyer.co.uk",
                Telephone ="020 555 6666 ext 2",
                ContactPreferenceType = 2,
                CreatedOn = DateTime.Parse("2022-10-01 10:11"),
                ModifiedOn = null
            }
        };

    public IList<EmployerInterest> BuildList(int numberToTake) =>
        BuildList().Take(numberToTake).ToList();
}