using System.Collections.Generic;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Extensions;

public static class MappingExtensions
{
    public static IEnumerable<LocationDto> MapToLocationDtoCollection(
        this IEnumerable<Location> locations,
        long ukPrn)
    {
        var results = new List<LocationDto>();

        if (locations is not null)
        {
            foreach (var location in locations)
            {
                results.Add(new LocationDto
                {
                    UkPrn = ukPrn,
                    Postcode = location.Postcode,
                    Name = location.Name,
                    AddressLine1 = location.AddressLine1,
                    AddressLine2 = location.AddressLine2,
                    Town = location.Town,
                    County = location.County,
                    Email = location.Email,
                    Telephone = location.Telephone,
                    Website = location.Website,
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    IsAdditionalData = location.IsAdditionalData
                });
            }
        }

        return results;
    }

    public static LocationDto MapToLocationDto(
        this Location location,
        long ukPrn)
    {
        if (location is null)
            return null;

        return new LocationDto
        {
            UkPrn = ukPrn,
            Postcode = location.Postcode,
            Name = location.Name,
            AddressLine1 = location.AddressLine1,
            AddressLine2 = location.AddressLine2,
            Town = location.Town,
            County = location.County,
            Email = location.Email,
            Telephone = location.Telephone,
            Website = location.Website,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            IsAdditionalData = location.IsAdditionalData
        };
    }

    public static IEnumerable<LocationQualificationDto> MapToLocationQualificationDtoList(
        this IEnumerable<DeliveryYear> deliveryYears,
        long ukPrn, 
        string postcode,
        bool isAdditionalData = false)
    {
        var results = new List<LocationQualificationDto>();

        if (deliveryYears is not null)
        {
            foreach (var deliveryYear in deliveryYears)
            {
                foreach (var qualification in deliveryYear.Qualifications)
                {
                    results.Add(new LocationQualificationDto
                    {
                        UkPrn = ukPrn,
                        Postcode = postcode,
                        DeliveryYear = deliveryYear.Year,
                        QualificationId = qualification.Id,
                        IsAdditionalData = isAdditionalData
                    });
                }
            }
        }

        return results;
    }
}