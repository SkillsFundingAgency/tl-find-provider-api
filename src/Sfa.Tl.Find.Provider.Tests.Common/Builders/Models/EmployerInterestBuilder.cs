using Castle.Components.DictionaryAdapter;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class EmployerInterestBuilder
{
    private IList<Guid> _uniqueIds = new List<Guid>();

    public IEnumerable<EmployerInterest> BuildList() =>
        new List<EmployerInterest>
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
                IndustryId = 9,
                OtherIndustry = null,
                AdditionalInformation = "These are my requirements: none",
                Email = "test.contact1@employer.co.uk",
                Telephone = "020 555 6666 ext 1",
                Website = "https://employer-one.co.uk",
                ContactPreferenceType = 1,
                SkillAreaIds = new List<int>{ 1, 2 }
            },
            new()
            {
                Id = 2,
                UniqueId = _uniqueIds.Skip(1).FirstOrDefault(),
                OrganisationName = "Test Employer 2",
                ContactName = "Test Contact 2",
                Postcode = "CV1 3XT",
                Latitude = 52.400997,
                Longitude = -1.508122,
                IndustryId = null,
                OtherIndustry = "Test Industry",
                AdditionalInformation = "These are my requirements: a few good people",
                Email = "test.contact2@employer.co.uk",
                Telephone = "020 555 6666 ext 2",
                Website = "https://employer-two.co.uk",
                ContactPreferenceType = 2,
                SkillAreaIds = new List<int>{ 1 }
            }
        };

    public IList<EmployerInterest> BuildList(int numberToTake) =>
        BuildList().Take(numberToTake).ToList();

    public EmployerInterest Build() =>
        BuildList().First();

    public EmployerInterestBuilder WithUniqueId(Guid uniqueId)
    {
        _uniqueIds.Clear();
        _uniqueIds.Add(uniqueId);

        return this;
    }

    public EmployerInterestBuilder WithUniqueIds(IEnumerable<Guid> uniqueIds)
    {
        _uniqueIds = uniqueIds.ToList();

        return this;
    }
}