using FluentAssertions;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Models;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Extensions;

public class MappingExtensionsTests
{
    [Fact]
    public void Null_Location_To_LocationDto_Returns_Expected_Result()
    {
        const long ukPrn = 12345678;

        var result = ((Location)null).MapToLocationDto(ukPrn);

        result.Should().BeNull();
    }

    [Fact]
    public void Location_To_LocationDto_Returns_Expected_Result()
    {
        const long ukPrn = 12345678;

        var location = new LocationBuilder().Build();

        var result = location.MapToLocationDto(ukPrn);

        result.Should().NotBeNull();

        result.UkPrn.Should().Be(ukPrn);
        result.Postcode.Should().Be(location.Postcode);
        result.Name.Should().Be(location.Name);
        result.AddressLine1.Should().Be(location.AddressLine1);
        result.AddressLine2.Should().Be(location.AddressLine2);
        result.Town.Should().Be(location.Town);
        result.County.Should().Be(location.County);
        result.Email.Should().Be(location.Email);
        result.Telephone.Should().Be(location.Telephone);
        result.Website.Should().Be(location.Website);
        result.Latitude.Should().Be(location.Latitude);
        result.Longitude.Should().Be(location.Longitude);
        result.IsAdditionalData.Should().Be(location.IsAdditionalData);
    }

    [Fact]
    public void Location_To_LocationDto_Returns_Expected_Result_For_Additional_Data()
    {
        const long ukPrn = 12345678;

        var location = new LocationBuilder().Build(true);

        var result = location.MapToLocationDto(ukPrn);

        result.Should().NotBeNull();

        result.UkPrn.Should().Be(ukPrn);
        result.Postcode.Should().Be(location.Postcode);
        result.Name.Should().Be(location.Name);
        result.AddressLine1.Should().Be(location.AddressLine1);
        result.AddressLine2.Should().Be(location.AddressLine2);
        result.Town.Should().Be(location.Town);
        result.County.Should().Be(location.County);
        result.Email.Should().Be(location.Email);
        result.Telephone.Should().Be(location.Telephone);
        result.Website.Should().Be(location.Website);
        result.Latitude.Should().Be(location.Latitude);
        result.Longitude.Should().Be(location.Longitude);
        result.IsAdditionalData.Should().Be(location.IsAdditionalData);
    }

    [Fact]
    public void Null_LocationCollection_To_LocationDtoCollection_Returns_Expected_Result()
    {
        const long ukPrn = 12345678;

        var result = ((IEnumerable<Location>)null).MapToLocationDtoCollection(ukPrn)?.ToList();

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void LocationCollection_To_LocationDtoCollection_Returns_Expected_Result()
    {
        const long ukPrn = 12345678;

        var locations = new LocationBuilder().BuildList().ToList();

        var result = locations.MapToLocationDtoCollection(ukPrn)?.ToList();

        result.Should().NotBeNull();
        result!.Count.Should().Be(locations.Count);

        for (var i = 0; i < result.Count; i++)
        {
            result[i].UkPrn.Should().Be(ukPrn);
            result[i].Postcode.Should().Be(locations[i].Postcode);
            result[i].Name.Should().Be(locations[i].Name);
            result[i].AddressLine1.Should().Be(locations[i].AddressLine1);
            result[i].AddressLine2.Should().Be(locations[i].AddressLine2);
            result[i].Town.Should().Be(locations[i].Town);
            result[i].County.Should().Be(locations[i].County);
            result[i].Email.Should().Be(locations[i].Email);
            result[i].Telephone.Should().Be(locations[i].Telephone);
            result[i].Website.Should().Be(locations[i].Website);
            result[i].Latitude.Should().Be(locations[i].Latitude);
            result[i].Longitude.Should().Be(locations[i].Longitude);
            result[i].IsAdditionalData.Should().Be(locations[i].IsAdditionalData);
        }
    }

    [Fact]
    public void LocationCollection_To_LocationDtoCollection_Returns_Expected_Result_For_Additional_Data()
    {
        const long ukPrn = 12345678;

        var locations = new LocationBuilder().BuildList(true).ToList();

        var result = locations.MapToLocationDtoCollection(ukPrn)?.ToList();

        result.Should().NotBeNull();
        result!.Count.Should().Be(locations.Count);

        for (var i = 0; i < result.Count; i++)
        {
            result[i].UkPrn.Should().Be(ukPrn);
            result[i].Postcode.Should().Be(locations[i].Postcode);
            result[i].Name.Should().Be(locations[i].Name);
            result[i].AddressLine1.Should().Be(locations[i].AddressLine1);
            result[i].AddressLine2.Should().Be(locations[i].AddressLine2);
            result[i].Town.Should().Be(locations[i].Town);
            result[i].County.Should().Be(locations[i].County);
            result[i].Email.Should().Be(locations[i].Email);
            result[i].Telephone.Should().Be(locations[i].Telephone);
            result[i].Website.Should().Be(locations[i].Website);
            result[i].Latitude.Should().Be(locations[i].Latitude);
            result[i].Longitude.Should().Be(locations[i].Longitude);
            result[i].IsAdditionalData.Should().Be(locations[i].IsAdditionalData);
        }
    }

    [Fact]
    public void Null_LocationQualificationCollection_To_LocationQualificationDtoCollection_Returns_Expected_Result()
    {
        const long ukPrn = 12345678;
        const string postcode = "CV1 2WT";

        var result = ((IEnumerable<DeliveryYear>)null).MapToLocationQualificationDtoList(ukPrn, postcode)?.ToList();

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void LocationQualificationCollection_To_LocationQualificationDtoCollection_Returns_Expected_Result()
    {
        const long ukPrn = 12345678;
        const string postcode = "CV1 2WT";
        var deliveryYears = new DeliveryYearBuilder().BuildList().ToList();

        var expectedNumberOfDeliveryYearsAndQualifications = deliveryYears.Sum(deliveryYear => deliveryYear.Qualifications.Count);

        var result = deliveryYears.MapToLocationQualificationDtoList(ukPrn, postcode)?.ToList();

        result.Should().NotBeNull();
        result!.Count.Should().Be(expectedNumberOfDeliveryYearsAndQualifications);

        foreach (var dto in result)
        {
            dto.UkPrn.Should().Be(ukPrn);
            dto.Postcode.Should().Be(postcode);
            dto.IsAdditionalData.Should().BeFalse();

            //Find the matching item...
            var item1 = dto;
            deliveryYears
                .Where(x => x.Year == item1.DeliveryYear && x.Qualifications.Any(q => q.Id == item1.QualificationId))
                .Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public void LocationQualificationCollection_To_LocationQualificationDtoCollection_Returns_Expected_Result_For_Additional_Data()
    {
        const long ukPrn = 12345678;
        const string postcode = "CV1 2WT";
        var deliveryYears = new DeliveryYearBuilder().BuildList().ToList();

        var expectedNumberOfDeliveryYearsAndQualifications = deliveryYears.Sum(deliveryYear => deliveryYear.Qualifications.Count);

        var result = deliveryYears.MapToLocationQualificationDtoList(ukPrn, postcode, true)?.ToList();

        result.Should().NotBeNull();
        result!.Count.Should().Be(expectedNumberOfDeliveryYearsAndQualifications);

        foreach (var dto in result)
        {
            dto.UkPrn.Should().Be(ukPrn);
            dto.Postcode.Should().Be(postcode);
            dto.IsAdditionalData.Should().BeTrue();

            //Find the matching item...
            var item1 = dto;
            deliveryYears
                .Where(x => x.Year == item1.DeliveryYear && x.Qualifications.Any(q => q.Id == item1.QualificationId))
                .Should().NotBeNullOrEmpty();
        }
    }
}