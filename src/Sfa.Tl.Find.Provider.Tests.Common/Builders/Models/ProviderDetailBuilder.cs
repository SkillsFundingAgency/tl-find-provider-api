using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class ProviderDetailBuilder
{
    public IEnumerable<ProviderDetail> BuildList() =>
        new List<ProviderDetail>
        {
            new()
            {
                UkPrn = 10000001,
                Name = "Provider 1",
                Postcode = "AA1 1AA",
                AddressLine1 = "Location 1 Address Line 1",
                AddressLine2 = "Location 1 Address Line 2",
                Town = "Location 1 Town",
                County = "Location 1 County",
                Email = "email.address@provider1.ac.uk",
                Telephone = "011 111 1111",
                Website = "https://www.provider1.ac.uk",
                Locations = new List<LocationDetail>
                {
                    new()
                    {
                        LocationName = "Location 1",
                        Postcode = "AA1 1BB",
                        LocationAddressLine1 = "Location 1 Address Line 1",
                        LocationAddressLine2 = "Location 1 Address Line 2",
                        Town = "Location 1 Town",
                        County = "Location 1 County",
                        Email = "location1.email.address@provider1.ac.uk",
                        Telephone = "011 111 2222",
                        Website = "https://www.provider1.ac.uk/location1",
                        DeliveryYears = new List<DeliveryYearDetail>
                        {
                            new()
                            {
                                Year = 2021,
                                IsAvailableNow = true,
                                Routes = new List<RouteDetail>
                                {
                                    new()
                                    {
                                        RouteId = 1,
                                        RouteName = "Test Route 1",
                                        Qualifications = new List<QualificationDetail>
                                        {
                                            new()
                                            {
                                                Id = 31,
                                                Name = "Test Qualification 31"
                                            },
                                            new()
                                            {
                                                Id = 32,
                                                Name = "Test Qualification 32"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new()
            {
                UkPrn = 10000002,
                Name = "Provider 2",
                Postcode = "BB2 2BB",
                AddressLine1 = "Provider 2 Address Line 1",
                AddressLine2 = "Provider 2 Address Line 2",
                Town = "Provider 2 Town",
                County = "Provider 2 County",
                Email = "email.address@provider2.ac.uk",
                Telephone = "022 222 2222",
                Website = "https://www.provider2.ac.uk",
                Locations = new List<LocationDetail>
                {
                    new()
                        {
                        LocationName = "Location 2",
                        Postcode = "AA1 1BB",
                        LocationAddressLine1 = "Location 2 Address Line 1",
                        LocationAddressLine2 = "Location 2 Address Line 2",
                        Town = "Location 2 Town",
                        County = "Location 2 County",
                        Email = "location2.email.address@provider2.ac.uk",
                        Telephone = "011 111 2222",
                        Website = "https://www.provider2.ac.uk/location2",
                        DeliveryYears = new List<DeliveryYearDetail>
                        {
                            new()
                            {
                                Year = 2022,
                                IsAvailableNow = true,
                                Routes = new List<RouteDetail>
                                {
                                    new()
                                    {
                                        RouteId = 2,
                                        RouteName = "Test Route 2",
                                        Qualifications = new List<QualificationDetail>
                                        {
                                            new()
                                            {
                                                Id = 51,
                                                Name = "Test Qualification 51"
                                            }
                                        }
                                    }
                                }
                            },
                            new()
                            {
                                Year = 2023,
                                IsAvailableNow = true,
                                Routes = new List<RouteDetail>
                                {
                                    new()
                                    {
                                        RouteId = 3,
                                        RouteName = "Test Route 3",
                                        Qualifications = new List<QualificationDetail>
                                        {
                                            new()
                                            {
                                                Id = 52,
                                                Name = "Test Qualification 52"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

    public IEnumerable<ProviderDetail> BuildListWithSingleItem() =>
        new List<ProviderDetail>
        {
            new()
            {
                UkPrn = 10000001,
                Name = "Provider 1",
                Postcode = "AA1 1AA",
                AddressLine1 = "Provider 1 Address Line 1",
                AddressLine2 = "Provider 1 Address Line 2",
                Town = "Provider 1 Town",
                County = "Provider 1 County",
                Email = "email.address@provider1.ac.uk",
                Telephone = "011 111 1111",
                Website = "https://www.provider1.ac.uk",
                Locations = new List<LocationDetail>
                {
                    new()
                    {
                        LocationName = "Location 1",
                        Postcode = "AA1 1BB",
                        LocationAddressLine1 = "Location 1 Address Line 1",
                        LocationAddressLine2 = "Location 1 Address Line 2",
                        Town = "Location 1 Town",
                        County = "Location 1 County",
                        Email = "location1.email.address@provider1.ac.uk",
                        Telephone = "011 111 2222",
                        Website = "https://www.provider1.ac.uk/location1",
                        DeliveryYears = new List<DeliveryYearDetail>
                        {
                            new()
                            {
                                Year = 2021,
                                IsAvailableNow = true,
                                Routes = new List<RouteDetail>
                                {
                                        new()
                                        {
                                            RouteId = 1,
                                            RouteName = "Test Route 1",
                                            Qualifications = new List<QualificationDetail>
                                            {
                                                new()
                                                {
                                                    Id = 31,
                                                    Name = "Test Qualification 31"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
        };

    public IEnumerable<ProviderDetail> BuildProvidersPartOfListWithSingleItem() =>
        new List<ProviderDetail>
        {
            new()
            {
                UkPrn = 10000001,
                Name = "Provider 1",
                Postcode = "AA1 1AA",
                AddressLine1 = "Provider 1 Address Line 1",
                AddressLine2 = "Provider 1 Address Line 2",
                Town = "Provider 1 Town",
                County = "Provider 1 County",
                Email = "email.address@provider1.ac.uk",
                Telephone = "011 111 1111",
                Website = "https://www.provider1.ac.uk"
            }
        };

    public IEnumerable<LocationDetail> BuildLocationsPartOfListWithSingleItem() =>
        new List<LocationDetail>
        {
            new()
            {
                LocationName = "Location 1",
                Postcode = "AA1 1BB",
                LocationAddressLine1 = "Location 1 Address Line 1",
                LocationAddressLine2 = "Location 1 Address Line 2",
                Town = "Location 1 Town",
                County = "Location 1 County",
                Email = "location1.email.address@provider1.ac.uk",
                Telephone = "011 111 2222",
                Website = "https://www.provider1.ac.uk/location1"
            }
        };

    public IEnumerable<DeliveryYearDetail> BuildDeliveryYearsPartOfListWithSingleItem() =>
        new List<DeliveryYearDetail>
        {
            new()
            {
                Year = 2021,
                IsAvailableNow = true
            }
        };

    public IEnumerable<RouteDetail> BuildRoutesPartOfListWithSingleItem() =>
        new List<RouteDetail>
        {
            new()
            {
                RouteId = 1,
                RouteName = "Test Route 1"
            }
        };

    public IEnumerable<QualificationDetail> BuildQualificationsPartOfListWithSingleItem() =>
        new List<QualificationDetail>
        {
                new()
                {
                    Id = 31,
                    Name = "Test Qualification 31"
                }
        };
}