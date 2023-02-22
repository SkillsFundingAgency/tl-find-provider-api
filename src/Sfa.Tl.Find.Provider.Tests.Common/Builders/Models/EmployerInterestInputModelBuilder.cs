using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class EmployerInterestInputModelBuilder
{
    public EmployerInterestInputModel Build() =>
        new()
        {
            Id = 1,
            OrganisationName = "Test Employer",
            ContactName = "Test Contact",
            //Postcode = "CV1 2WT",
            IndustryId = 9,
            OtherIndustry = "Test Industry",
            AdditionalInformation = "These are my requirements: none",
            Email = "test.contact1@employer.co.uk",
            Telephone = "020 555 6666 ext 1",
            Website = "https://employer-one.co.uk",
            ContactPreferenceType = ContactPreference.Email,
            Locations = new List<NamedLocation>
            {
                new()
                {
                    Name = "Main location",
                    Postcode = "CV1 2WT"
                }
            },
            SkillAreaIds = new List<int> { 1, 2 }
        };
}