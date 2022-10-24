using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class EmployerInterestDetailBuilder
{
    private Guid _uniqueId;
    private List<string> _skillAreas = new List<string>();

    public EmployerInterestDetail Build() => new()
    {
        Id = 1,
        UniqueId = _uniqueId,
        OrganisationName = "Test Employer",
        ContactName = "Test Contact",
        Postcode = "CV1 2WT",
        Latitude = 52.400997,
        Longitude = -1.508122,
        Industry = "Digital and IT",
        AdditionalInformation = "These are my requirements: none",
        Email = "test.contact1@employer.co.uk",
        Telephone = "020 555 6666 ext 1",
        Website = "https://employer-one.co.uk",
        ContactPreferenceType = ContactPreference.Email,
        CreatedOn = DateTime.Parse("2022-10-24 12:00"),
        ModifiedOn = DateTime.Parse("2022-11-05 15:00"),
        SkillAreas = _skillAreas
    };

    public EmployerInterestDetailBuilder WithSkillAreas(IList<string> skillAreas)
    {
        _skillAreas.Clear();
        _skillAreas.AddRange(skillAreas);

        return this;
    }

    public EmployerInterestDetailBuilder WithUniqueId(Guid uniqueId)
    {
        _uniqueId = uniqueId;

        return this;
    }
}