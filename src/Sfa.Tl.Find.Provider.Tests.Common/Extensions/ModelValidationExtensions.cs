﻿using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Extensions;

public static class ModelValidationExtensions
{
    public const double DoubleTolerance = 0.00001;

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
        string employerContactEmail = null,
        string employerContactTelephone = null,
        string employerContactWebsite = null,
        string studentContactEmail = null,
        string studentContactTelephone = null,
        string studentContactWebsite = null,
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

        provider.EmployerContactEmail.Should().Be(employerContactEmail);
        provider.EmployerContactTelephone.Should().Be(employerContactTelephone);
        provider.EmployerContactWebsite.Should().Be(employerContactWebsite);
        provider.StudentContactEmail.Should().Be(studentContactEmail);
        provider.StudentContactTelephone.Should().Be(studentContactTelephone);
        provider.StudentContactWebsite.Should().Be(studentContactWebsite);

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

        provider.Locations.Should().NotBeNull();
        provider.Locations.Count.Should().Be(expected.Locations.Count);

        foreach (var location in provider.Locations)
        {
            var expectedLocation = expected.Locations.Single(l => l.Postcode == location.Postcode);
            location.Validate(expectedLocation);
        }
    }

    public static void Validate(this ProviderDetailFlat provider, ProviderDetailFlat expected)
    {
        provider.UkPrn.Should().Be(expected.UkPrn);
        provider.ProviderName.Should().Be(expected.ProviderName);
        provider.Postcode.Should().Be(expected.Postcode);
        provider.LocationName.Should().Be(expected.LocationName);
        provider.AddressLine1.Should().Be(expected.AddressLine1);
        provider.AddressLine2.Should().Be(expected.AddressLine2);
        provider.Town.Should().Be(expected.Town);
        provider.County.Should().Be(expected.County);
        provider.Email.Should().Be(expected.Email);
        provider.Telephone.Should().Be(expected.Telephone);
        provider.Website.Should().Be(expected.Website);
        provider.Year.Should().Be(expected.Year);
        provider.RouteId.Should().Be(expected.RouteId);
        provider.RouteName.Should().Be(expected.RouteName);
        provider.QualificationId.Should().Be(expected.QualificationId);
        provider.QualificationName.Should().Be(expected.QualificationName);
    }

    public static void Validate(this ProviderContactDto provider,
        ProviderContactDto expected)
    {
        provider.Should().NotBeNull();
        provider.UkPrn.Should().Be(expected.UkPrn);
        provider.Name.Should().Be(expected.Name);
        provider.EmployerContactEmail.Should().Be(expected.EmployerContactEmail);
        provider.EmployerContactTelephone.Should().Be(expected.EmployerContactTelephone);
        provider.EmployerContactWebsite.Should().Be(expected.EmployerContactWebsite);
        provider.StudentContactEmail.Should().Be(expected.StudentContactEmail);
        provider.StudentContactTelephone.Should().Be(expected.StudentContactTelephone);
        provider.StudentContactWebsite.Should().Be(expected.StudentContactWebsite);
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
        location.LocationName.Should().Be(expected.LocationName);
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

    public static void Validate(this LocationDto location, Location expected, long expectedUkPrn)
    {
        location.UkPrn.Should().Be(expectedUkPrn);
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
    }

    public static void Validate(this LocationPostcode locationPostcode, LocationPostcode expected)
    {
        locationPostcode.Id.Should().Be(expected.Id);
        locationPostcode.Postcode.Should().Be(expected.Postcode);
        locationPostcode.Name.Should().Be(expected.Name);
        locationPostcode.Latitude.Should().Be(expected.Latitude);
        locationPostcode.Longitude.Should().Be(expected.Longitude);
    }

    public static void Validate(this LocationPostcode locationPostcode, LocationPostcodeDto expected)
    {
        locationPostcode.Id.Should().Be(expected.LocationId);
        locationPostcode.Name.Should().Be(expected.LocationName);
        locationPostcode.Postcode.Should().Be(expected.Postcode);
    }

    public static void Validate(this Route route, RouteDto expected)
    {
        route.Id.Should().Be(expected.RouteId);
        route.Name.Should().Be(expected.RouteName);
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
            var expectedRoute = expected.Routes.Single(r => r.RouteId == route.RouteId);
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
        route.RouteId.Should().Be(expected.RouteId);
        route.RouteName.Should().Be(expected.RouteName);

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

    public static void Validate(this Qualification qualification, QualificationDto expected)
    {
        qualification.Id.Should().Be(expected.QualificationId);
        qualification.Name.Should().Be(expected.QualificationName);
    }

    public static bool Validate(this EmployerInterest employerInterest, EmployerInterest expected,
        bool validateId = false,
        bool validateUniqueId = false,
        bool validatePostcode = true,
        bool validateLatLong = true,
        bool validateExpiry = true)
    {
        if (validateId)
        {
            employerInterest.Id.Should().Be(expected.Id);
        }

        if (validateUniqueId)
        {
            employerInterest.UniqueId.Should().Be(expected.UniqueId);
        }

        employerInterest.OrganisationName.Should().Be(expected.OrganisationName);
        employerInterest.ContactName.Should().Be(expected.ContactName);

        employerInterest.ContactPreferenceType.Should().Be(expected.ContactPreferenceType);
        employerInterest.Email.Should().Be(expected.Email);
        employerInterest.Telephone.Should().Be(expected.Telephone);
        employerInterest.Website.Should().Be(expected.Website);

        if (validateExpiry)
        {
            employerInterest.ExpiryDate.Should().Be(expected.ExpiryDate);
        }
        employerInterest.ExtensionCount.Should().Be(expected.ExtensionCount);

        if (validatePostcode)
        {
            employerInterest.Postcode.Should().Be(expected.Postcode);
        }

        if (validateLatLong)
        {
            employerInterest.Longitude.Should().BeApproximately(expected.Longitude, DoubleTolerance);
            employerInterest.Latitude.Should().BeApproximately(expected.Latitude, DoubleTolerance);
        }

        employerInterest.IndustryId.Should().Be(expected.IndustryId);
        employerInterest.OtherIndustry.Should().Be(expected.OtherIndustry);

        employerInterest.AdditionalInformation.Should().Be(expected.AdditionalInformation);

        return true;
    }

    public static bool Validate(this GeoLocation geoLocation, GeoLocation expected)
    {
        geoLocation.Location.Should().Be(expected.Location);
        geoLocation.Longitude.Should().BeApproximately(expected.Longitude, DoubleTolerance);
        geoLocation.Latitude.Should().BeApproximately(expected.Latitude, DoubleTolerance);

        return true;
    }

    public static void Validate(this Town town,
        int id,
        string name,
        string county,
        string localAuthority,
        decimal latitude,
        decimal longitude)
    {
        town.Should().NotBeNull();
        town.Id.Should().Be(id);
        town.Name.Should().Be(name);
        town.County.Should().Be(county);
        town.LocalAuthority.Should().Be(localAuthority);
        town.Latitude.Should().Be(latitude);
        town.Longitude.Should().Be(longitude);
    }

    public static void Validate(this EmployerInterestSummary employerInterestSummary, EmployerInterestSummaryDto expected)
    {
        employerInterestSummary.Id.Should().Be(expected.Id);
        employerInterestSummary.OrganisationName.Should().Be(expected.OrganisationName);
        employerInterestSummary.Industry.Should().Be(expected.Industry);
        employerInterestSummary.Distance.Should().Be(expected.Distance);
        employerInterestSummary.ExpiryDate.Should().Be(expected.ExpiryDate);
        employerInterestSummary.CreatedOn.Should().Be(expected.CreatedOn);
        employerInterestSummary.ModifiedOn.Should().Be(expected.ModifiedOn);
    }

    public static void Validate(this SearchFilter searchFilter, SearchFilterDto expected)
    {
        searchFilter.Id.Should().Be(expected.Id);
        searchFilter.LocationId.Should().Be(expected.LocationId);
        searchFilter.LocationName.Should().Be(expected.LocationName);
        searchFilter.Postcode.Should().Be(expected.Postcode);
        searchFilter.SearchRadius.Should().Be(expected.SearchRadius);
    }

    public static void Validate(this Notification notification, NotificationDto expected)
    {
        notification.Id.Should().Be(expected.Id);
        notification.Email.Should().Be(expected.Email);
        notification.IsEmailVerified.Should().Be(expected.IsEmailVerified);
        notification.EmailVerificationToken.Should().Be(expected.EmailVerificationToken);
        notification.Frequency.Should().Be(expected.Frequency);
        notification.LocationId.Should().Be(expected.LocationId);
        notification.LocationName.Should().Be(expected.LocationName);
        notification.Postcode.Should().Be(expected.Postcode);
        notification.SearchRadius.Should().Be(expected.SearchRadius);
    }

    public static void Validate(this NotificationSummary notificationSummary, NotificationSummaryDto expected)
    {
        notificationSummary.Id.Should().Be(expected.Id);
        notificationSummary.Email.Should().Be(expected.Email);
        notificationSummary.IsEmailVerified.Should().Be(expected.IsEmailVerified);
    }

    public static void Validate(this NotificationLocationSummary notificationLocationSummary, NotificationLocationSummaryDto expected)
    {
        notificationLocationSummary.Id.Should().Be(expected.Id);
        notificationLocationSummary.Frequency.Should().Be(expected.Frequency);
        notificationLocationSummary.SearchRadius.Should().Be(expected.SearchRadius);

        notificationLocationSummary.Location?.Id.Should().Be(expected.LocationId);
        notificationLocationSummary.Location?.Name.Should().Be(expected.LocationName);
        notificationLocationSummary.Location?.Postcode.Should().Be(expected.Postcode);
    }

    public static void Validate(this NotificationLocationName locationPostcode, NotificationLocationNameDto expected)
    {
        locationPostcode.Id.Should().Be(expected.NotificationLocationId);
        locationPostcode.LocationId.Should().Be(expected.LocationId);
        locationPostcode.Postcode.Should().Be(expected.Postcode);
        locationPostcode.Name.Should().Be(expected.LocationName);
    }
}