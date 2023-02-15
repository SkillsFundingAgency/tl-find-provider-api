using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class EmployerInterestBuilder
{
    private string _additionalInformation;
    private readonly List<int> _extensionCounts = new() { 0, 0 };
    private readonly List<int?> _industryIds 
        = new() { 9, null };
    private readonly List<string> _otherIndustries 
        = new() { null, "Test Industry" };
    private readonly List<(double, double)> _latitudesAndLongitudes 
        = new()
        {
            (52.400997, -1.508122),
            (52.406587, -1.523157)
        };
    private readonly List<Guid> _uniqueIds = new();
      private readonly List<int> _skillAreaIds 
        = new();
  
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
                Latitude = GetLatitude(0),
                Longitude = GetLongitude(0),
                IndustryId = _industryIds.FirstOrDefault(),
                OtherIndustry = _otherIndustries.FirstOrDefault(),
                AdditionalInformation = _additionalInformation ?? "These are my requirements: none",
                Email = "test.contact1@employer.co.uk",
                Telephone = "020 555 6666 ext 1",
                Website = "https://employer-one.co.uk",
                ContactPreferenceType = ContactPreference.Email,
                ExpiryDate = DateTime.Parse("2023-03-31"),
                ExtensionCount = GetExtensionCount(0),
                SkillAreaIds = _skillAreaIds.Any() ? _skillAreaIds : new List<int>{ 1, 2 }
            },
            new()
            {
                Id = 2,
                UniqueId = _uniqueIds.Skip(1).FirstOrDefault(),
                OrganisationName = "Test Employer 2",
                ContactName = "Test Contact 2",
                Postcode = "CV1 3GT",
                Latitude = GetLatitude(1),
                Longitude = GetLongitude(1),
                IndustryId = _industryIds.Skip(1).FirstOrDefault(),
                OtherIndustry = _otherIndustries.Skip(1).FirstOrDefault(),
                AdditionalInformation = _additionalInformation ?? "These are my requirements: a few good people",
                Email = "test.contact2@employer.co.uk",
                Telephone = "020 555 6666 ext 2",
                Website = "https://employer-two.co.uk",
                ContactPreferenceType = ContactPreference.Telephone,
                ExpiryDate = DateTime.Parse("2023-03-31"),
                ExtensionCount = GetExtensionCount(1),
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

    public EmployerInterestBuilder WithLatLong(double latitude, double longitude)
    {
        _latitudesAndLongitudes.Clear();
        _latitudesAndLongitudes.Add((latitude, longitude));

        return this;
    }

    public EmployerInterestBuilder WithLatLongs(IEnumerable<(double, double)> latLongs)
    {
        _latitudesAndLongitudes.Clear();
        _latitudesAndLongitudes.AddRange(latLongs);

        return this;
    }

    public EmployerInterestBuilder WithExtensionCounts(IEnumerable<int> extensionCounts)
    {
        _extensionCounts.Clear();
        _extensionCounts.AddRange(extensionCounts);

        return this;
    }

    public EmployerInterestBuilder WithIndustryId(int? industryId)
    {
        _industryIds.Clear();
        _industryIds.Add(industryId);

        return this;
    }

    public EmployerInterestBuilder WithIndustryIds(IEnumerable<int?> industryIds)
    {
        _industryIds.Clear();
        _industryIds.AddRange(industryIds);

        return this;
    }

    public EmployerInterestBuilder WithOtherIndustry(string otherIndustry)
    {
        _otherIndustries.Clear();
        _otherIndustries.Add(otherIndustry);

        return this;
    }

    public EmployerInterestBuilder WithOtherIndustries(IEnumerable<string> otherIndustries)
    {
        _otherIndustries.Clear();
        _otherIndustries.AddRange(otherIndustries);

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

    public EmployerInterestBuilder WithUniqueId(Guid uniqueId)
    {
        _uniqueIds.Clear();
        _uniqueIds.Add(uniqueId);

        return this;
    }

    private int GetExtensionCount(int index) =>
        _extensionCounts.Count > index
            ? _extensionCounts[index]
            : 0;

    private double GetLatitude(int index) => 
        _latitudesAndLongitudes.Count > index 
            ? _latitudesAndLongitudes[index].Item1 
            : 0;

    private double GetLongitude(int index) => 
        _latitudesAndLongitudes.Count > index 
            ? _latitudesAndLongitudes[index].Item2 
            : 0;
}