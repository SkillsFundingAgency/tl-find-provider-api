﻿using FluentAssertions;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Extensions;

public static class ModelValidationExtensions
{
    public static void Validate(this Application.Models.Provider provider,
        long ukPrn,
        string name,
        string addressLine1,
        string addressLine2,
        string town,
        string county,
        string postcode,
        string email,
        string telephone,
        string website,
        int locationCount = 0)
    {
        provider.UkPrn.Should().Be(ukPrn);
        provider.Name.Should().Be(name);
        provider.AddressLine1.Should().Be(addressLine1);
        provider.AddressLine2.Should().Be(addressLine2);
        provider.Town.Should().Be(town);
        provider.County.Should().Be(county);
        provider.Postcode.Should().Be(postcode);
        provider.Email.Should().Be(email);
        provider.Telephone.Should().Be(telephone);
        provider.Website.Should().Be(website);

        provider.Locations.Should().NotBeNull();
        provider.Locations.Should().HaveCount(locationCount);
    }
 
    public static void Validate(this ProviderSearchResult result, ProviderSearchResult expected)
    {
        result.UkPrn.Should().Be(expected.UkPrn);
        result.ProviderName.Should().Be(expected.ProviderName);
        result.Postcode.Should().Be(expected.Postcode);
        result.LocationName.Should().Be(expected.LocationName);
        result.AddressLine1.Should().Be(expected.AddressLine1);
        result.AddressLine2.Should().Be(expected.AddressLine2);
        result.Town.Should().Be(expected.Town);
        result.County.Should().Be(expected.County);
        result.Email.Should().Be(expected.Email);
        result.Telephone.Should().Be(expected.Telephone);
        result.Website.Should().Be(expected.Website);
        result.Distance.Should().Be(expected.Distance);
        result.JourneyToLink.Should().Be(expected.JourneyToLink);

        result.DeliveryYears.Should().NotBeNull();
        result.DeliveryYears.Count.Should().Be(expected.DeliveryYears.Count);

        foreach (var deliveryYear in result.DeliveryYears)
        {
            var expectedDeliveryYear = expected.DeliveryYears.Single(dy => dy.Year == deliveryYear.Year);
            deliveryYear.Validate(expectedDeliveryYear);
        }
    }

    public static void Validate(this ProviderDetail provider, ProviderDetail expected)
    {
        provider.UkPrn.Should().Be(expected.UkPrn);
        provider.Name.Should().Be(expected.Name);
        provider.Postcode.Should().Be(expected.Postcode);
        provider.AddressLine1.Should().Be(expected.AddressLine1);
        provider.AddressLine2.Should().Be(expected.AddressLine2);
        provider.Town.Should().Be(expected.Town);
        provider.County.Should().Be(expected.County);
        provider.Email.Should().Be(expected.Email);
        provider.Telephone.Should().Be(expected.Telephone);
        provider.Website.Should().Be(expected.Website);
        provider.IsAdditionalData.Should().Be(expected.IsAdditionalData);

        provider.Locations.Should().NotBeNull();
        provider.Locations.Count.Should().Be(expected.Locations.Count);

        foreach (var location in provider.Locations)
        {
            var expectedLocation = expected.Locations.Single(l => l.Postcode == location.Postcode);
            location.Validate(expectedLocation);
        }
    }

    public static void Validate(this Location location,
        string name,
        string postcode,
        string addressLine1,
        string addressLine2,
        string town,
        string county,
        string email,
        string telephone,
        string website,
        double latitude,
        double longitude,
        int deliveryYearCount = 0)
    {
        location.Name.Should().Be(name);

        location.Postcode.Should().Be(postcode);
        location.AddressLine1.Should().Be(addressLine1);
        location.AddressLine2.Should().Be(addressLine2);
        location.Town.Should().Be(town);
        location.County.Should().Be(county);
        location.Email.Should().Be(email);
        location.Telephone.Should().Be(telephone);
        location.Website.Should().Be(website);

        location.Latitude.Should().Be(latitude);
        location.Longitude.Should().Be(longitude);

        location.DeliveryYears.Should().NotBeNull();
        location.DeliveryYears.Should().HaveCount(deliveryYearCount);
    }

    public static void Validate(this LocationDetail location, LocationDetail expected)
    {
        location.Postcode.Should().Be(expected.Postcode);
        location.Name.Should().Be(expected.Name);
        location.AddressLine1.Should().Be(expected.AddressLine1);
        location.AddressLine2.Should().Be(expected.AddressLine2);
        location.Town.Should().Be(expected.Town);
        location.County.Should().Be(expected.County);
        location.Email.Should().Be(expected.Email);
        location.Telephone.Should().Be(expected.Telephone);
        location.Website.Should().Be(expected.Website);
        location.Latitude.Should().Be(expected.Latitude);
        location.Longitude.Should().Be(expected.Longitude);

        location.DeliveryYears.Should().NotBeNull();
        location.DeliveryYears.Count.Should().Be(expected.DeliveryYears.Count);

        foreach (var deliveryYear in location.DeliveryYears)
        {
            var expectedDeliveryYear = expected.DeliveryYears.Single(dy => dy.Year == deliveryYear.Year);
            deliveryYear.Validate(expectedDeliveryYear);
        }
    }

    public static void Validate(this DeliveryYearSearchResult deliveryYear, DeliveryYearSearchResult expected)
    {
        deliveryYear.Year.Should().Be(expected.Year);
        deliveryYear.IsAvailableNow.Should().Be(expected.IsAvailableNow);
        deliveryYear.Routes.Should().NotBeNull();
        deliveryYear.Routes.Count.Should().Be(expected.Routes.Count);

        foreach (var route in deliveryYear.Routes)
        {
            var expectedRoute = expected.Routes.Single(r => r.Id == route.Id);
            route.Validate(expectedRoute);
        }
    }

    public static void Validate(this DeliveryYear deliveryYear, short year, IReadOnlyCollection<int> qualificationIds)
    {
        deliveryYear.Year.Should().Be(year);

        deliveryYear.Qualifications.Should().NotBeNull();
        deliveryYear.Qualifications.Should().HaveCount(qualificationIds.Count);

        foreach (var qualificationId in qualificationIds)
        {
            deliveryYear.Qualifications.Should().Contain(q => q.Id == qualificationId);
        }
    }

    public static void Validate(this DeliveryYearDetail deliveryYear, DeliveryYearDetail expected)
    {
        deliveryYear.Year.Should().Be(expected.Year);
        deliveryYear.IsAvailableNow.Should().Be(expected.IsAvailableNow);
        deliveryYear.Routes.Should().NotBeNull();
        deliveryYear.Routes.Count.Should().Be(expected.Routes.Count);

        foreach (var route in deliveryYear.Routes)
        {
            var expectedRoute = expected.Routes.Single(r => r.Id == route.Id);
            route.Validate(expectedRoute);
        }
    }

    public static void Validate(this Route route, Route expected)
    {
        route.Id.Should().Be(expected.Id);
        route.Name.Should().Be(expected.Name);

        foreach (var qualification in route.Qualifications)
        {
            var expectedQualification = expected.Qualifications.Single(q => q.Id == qualification.Id);
            qualification.Validate(expectedQualification);
        }
    }

    public static void Validate(this RouteDetail route, RouteDetail expected)
    {
        route.Id.Should().Be(expected.Id);
        route.Name.Should().Be(expected.Name);

        foreach (var qualification in route.Qualifications)
        {
            var expectedQualification = expected.Qualifications.Single(q => q.Id == qualification.Id);
            qualification.Validate(expectedQualification);
        }
    }

    public static void Validate(this Qualification qualification, Qualification expected)
    {
        qualification.Id.Should().Be(expected.Id);
        qualification.Name.Should().Be(expected.Name);
    }

    public static void Validate(this QualificationDetail qualification, QualificationDetail expected)
    {
        qualification.Id.Should().Be(expected.Id);
        qualification.Name.Should().Be(expected.Name);
    }
}