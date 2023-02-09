using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class EmployerInterestDetailBuilder
{
    private IList<Guid> _uniqueIds = new List<Guid>();
    private readonly List<string> _skillAreas = new();

    public EmployerInterestDetail Build() => 
        BuildList()
            .First();

    public IEnumerable<EmployerInterestDetail> BuildList() =>
        new List<EmployerInterestDetail>
        {
            new()
            {
                Id = 1,
                UniqueId = _uniqueIds.FirstOrDefault(),
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
                SkillAreas = _skillAreas,
                ExpiryDate = DateTime.Parse("2023-03-31"),
                ExtensionCount = 1,
                CreatedOn = DateTime.Parse("2022-10-24 12:00"),
                ModifiedOn = DateTime.Parse("2022-11-05 15:00")
            },
            new()
            {
                Id = 2,
                UniqueId = _uniqueIds.Skip(1).FirstOrDefault(),
                OrganisationName = "Test Employer 2",
                ContactName = "Test Contact 2",
                Postcode = "CV1 3GT",
                Latitude = 52.406587,
                Longitude = -1.523157,
                Industry = "Test Industry",
                AdditionalInformation = "These are my requirements: a few good people",
                Email = "test.contact2@employer.co.uk",
                Telephone = "020 555 6666 ext 2",
                Website = "https://employer-two.co.uk",
                ContactPreferenceType = ContactPreference.Telephone,
                SkillAreas = _skillAreas,
                ExpiryDate = DateTime.Parse("2023-03-31"),
                ExtensionCount = 0,
                CreatedOn = DateTime.Parse("2022-11-12 12:00"),
                ModifiedOn = null
            }
        };
    
    public EmployerInterestDetailBuilder WithSkillAreas(IList<string> skillAreas)
    {
        _skillAreas.Clear();
        _skillAreas.AddRange(skillAreas);

        return this;
    }

    public EmployerInterestDetailBuilder WithUniqueId(Guid uniqueId)
    {
        _uniqueIds.Clear();
        _uniqueIds.Add(uniqueId);

        return this;
    }

    public EmployerInterestDetailBuilder WithUniqueIds(IEnumerable<Guid> uniqueIds)
    {
        _uniqueIds = uniqueIds.ToList();

        return this;
    }
}