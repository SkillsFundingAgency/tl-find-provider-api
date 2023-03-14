using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

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
        result.Validate(location, ukPrn);
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
            result[i].Validate(locations[i], ukPrn);
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

        var result = deliveryYears.MapToLocationQualificationDtoList(ukPrn, postcode)?.ToList();

        result.Should().NotBeNull();
        result!.Count.Should().Be(expectedNumberOfDeliveryYearsAndQualifications);

        foreach (var dto in result)
        {
            dto.UkPrn.Should().Be(ukPrn);
            dto.Postcode.Should().Be(postcode);

            //Find the matching item...
            var item1 = dto;
            deliveryYears
                .Where(x => x.Year == item1.DeliveryYear && x.Qualifications.Any(q => q.Id == item1.QualificationId))
                .Should().NotBeNullOrEmpty();
        }
    }
}