using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class ProviderDetailFlatBuilder
{
    public IEnumerable<ProviderDetailFlat> BuildList() =>
        new List<ProviderDetailFlat>
        {
            new()
            {
                UkPrn = 10000001,
                ProviderName = "Provider 1",
                Postcode = "AA1 1AA",
                LocationName = "Location 1",
                AddressLine1 = "Location 1 Address Line 1",
                AddressLine2 = "Location 1 Address Line 2",
                Town = "Location 1 Town",
                County = "Location 1 County",
                Email = "email.address@provider1.ac.uk",
                Telephone = "011 111 1111",
                Website = "https://www.provider1.ac.uk/",
                Year = 2021,
                RouteId = 1,
                RouteName = "Test Route 1",
                QualificationId = 31,
                QualificationName = "Test Qualification 31"
            },
            new()
            {
                UkPrn = 10000001,
                ProviderName = "Provider 1",
                Postcode = "AA1 1AA",
                LocationName = "Location 1",
                AddressLine1 = "Location 1 Address Line 1",
                AddressLine2 = "Location 1 Address Line 2",
                Town = "Location 1 Town",
                County = "Location 1 County",
                Email = "email.address@provider1.ac.uk",
                Telephone = "011 111 1111",
                Website = "https://www.provider1.ac.uk/",
                Year = 2021,
                RouteId = 1,
                RouteName = "Test Route 1",
                QualificationId = 32,
                QualificationName = "Test Qualification 32"
            },
            new()
            {
                UkPrn = 10000002,
                ProviderName = "Provider 2",
                Postcode = "AA1 1BB",
                LocationName = "Location 2",
                AddressLine1 = "Location 2 Address Line 1",
                AddressLine2 = "Location 2 Address Line 2",
                Town = "Location 2 Town",
                County = "Location 2 County",
                Email = "location2.email.address@provider2.ac.uk",
                Telephone = "011 111 2222",
                Website = "https://www.provider2.ac.uk/location2",
                Year = 2022,
                RouteId = 2,
                RouteName = "Test Route 2",
                QualificationId = 51,
                QualificationName = "Test Qualification 51"
            },
            new()
            {
                UkPrn = 10000002,
                ProviderName = "Provider 2",
                Postcode = "AA1 1BB",
                LocationName = "Location 2",
                AddressLine1 = "Location 2 Address Line 1",
                AddressLine2 = "Location 2 Address Line 2",
                Town = "Location 2 Town",
                County = "Location 2 County",
                Email = "location2.email.address@provider2.ac.uk",
                Telephone = "011 111 2222",
                Website = "https://www.provider2.ac.uk/location2",
                Year = 2023,
                RouteId = 3,
                RouteName = "Test Route 3",
                QualificationId = 52,
                QualificationName = "Test Qualification 52"
            }
        };

    public IEnumerable<ProviderDetailFlat> BuildListWithSingleItem() =>
        new List<ProviderDetailFlat>
        {
            new()
            {
                UkPrn = 10000001,
                ProviderName = "Provider 1",
                Postcode = "AA1 1AA",
                LocationName = "Location 1",
                AddressLine1 = "Location 1 Address Line 1",
                AddressLine2 = "Location 1 Address Line 2",
                Town = "Location 1 Town",
                County = "Location 1 County",
                Email = "email.address@provider1.ac.uk",
                Telephone = "011 111 1111",
                Website = "https://www.provider1.ac.uk/",
                Year = 2021,
                RouteId = 1,
                RouteName = "Test Route 1",
                QualificationId = 31,
                QualificationName = "Test Qualification 31"
            }
        };
}