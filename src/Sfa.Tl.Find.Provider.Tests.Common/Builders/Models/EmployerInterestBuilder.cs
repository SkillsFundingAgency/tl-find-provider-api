using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class EmployerInterestBuilder
{
    private readonly List<Guid> _uniqueIds = new();
    private readonly List<int> _skillAreaIds = new();

    private string _additionalInformation;

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
                AdditionalInformation = _additionalInformation ?? "These are my requirements: none",
                Email = "test.contact1@employer.co.uk",
                Telephone = "020 555 6666 ext 1",
                Website = "https://employer-one.co.uk",
                ContactPreferenceType = ContactPreference.Email,
                SkillAreaIds = _skillAreaIds.Any() ? _skillAreaIds : new List<int>{ 1, 2 }
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
                AdditionalInformation = _additionalInformation ?? "These are my requirements: a few good people",
                Email = "test.contact2@employer.co.uk",
                Telephone = "020 555 6666 ext 2",
                Website = "https://employer-two.co.uk",
                ContactPreferenceType = ContactPreference.Telephone,
                SkillAreaIds = _skillAreaIds.Any() ? _skillAreaIds : new List<int>{ 1 }
            }
        };

    public IList<EmployerInterest> BuildList(int numberToTake) =>
        BuildList().Take(numberToTake).ToList();

    public EmployerInterest Build() =>
        BuildList().First();

    public EmployerInterest BuildWithEmptyNonMandatoryProperties()
    {
        return new EmployerInterest
        {
            Id = default,
            UniqueId = default,
            OrganisationName = "Test Employer",
            ContactName = "Test Contact",
            Postcode = "CV1 2WT",
            IndustryId = 9,
            OtherIndustry = null,
            Email = "test.contact1@employer.co.uk",
            Telephone = null,
            Website = null,
            ContactPreferenceType = default,
            AdditionalInformation = null,
            SkillAreaIds = _skillAreaIds.Any() ? _skillAreaIds : new List<int> { 1 }
        };
    }

    public EmployerInterest BuildWithGeoLocation(GeoLocation geoLocation, bool includeIds = false)
    {
        var employerInterest = Build();
        return new EmployerInterest
        {
            Id = includeIds ? employerInterest.Id : default,
            UniqueId = includeIds ? employerInterest.UniqueId : default,
            OrganisationName = employerInterest.OrganisationName,
            ContactName = employerInterest.ContactName,
            Postcode = geoLocation.Location,
            Latitude = geoLocation.Latitude,
            Longitude = geoLocation.Longitude,
            IndustryId = employerInterest.IndustryId,
            OtherIndustry = employerInterest.OtherIndustry,
            Email = employerInterest.Email,
            Telephone = employerInterest.Telephone,
            Website = employerInterest.Website,
            ContactPreferenceType = employerInterest.ContactPreferenceType,
            AdditionalInformation = employerInterest.AdditionalInformation,
            SkillAreaIds = employerInterest.SkillAreaIds
        };
    }


    public EmployerInterestBuilder WithAdditionalInformation(string additionalInformation)
    {
        _additionalInformation = additionalInformation;

        return this;
    }

    public EmployerInterestBuilder WithUniqueId(Guid uniqueId)
    {
        _uniqueIds.Clear();
        _uniqueIds.Add(uniqueId);

        return this;
    }

    public EmployerInterestBuilder WithUniqueIds(IEnumerable<Guid> uniqueIds)
    {
        _uniqueIds.Clear();
        _uniqueIds.AddRange(uniqueIds);

        return this;
    }

    public EmployerInterestBuilder WithSkillAreaIds(IList<int> skillAreaIds)
    {
        _skillAreaIds.Clear();
        _skillAreaIds.AddRange(skillAreaIds);

        return this;
    }
}