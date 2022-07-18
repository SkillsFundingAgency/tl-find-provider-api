using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class ProviderSearchResultBuilder
{
    private GeoLocation? _searchOrigin;

    public IEnumerable<ProviderSearchResult> BuildList() =>
        new List<ProviderSearchResult>
        {
            new()
            {
                UkPrn = 10000001,
                ProviderName = "Provider 1",
                LocationName = "Location 1",
                Postcode = "AA1 1AA",
                AddressLine1 = "Location 1 Address Line 1",
                AddressLine2 = "Location 1 Address Line",
                Town = "Location 1 Town",
                County = "Location 1 County",
                Email = "email.address@provider1.ac.uk",
                Telephone = "011 111 1111",
                Website = "https://www.provider1.ac.uk",
                Distance = 10.0,
                JourneyToLink = _searchOrigin?.CreateJourneyLink(new GeoLocation {Location = "AA1 1AA"}),
                DeliveryYears = new List<DeliveryYearSearchResult>
                {
                    new()
                    {
                        Year = 2021,
                        IsAvailableNow = true,
                        Routes = new List<Route>
                        {
                            new()
                            {
                                Id = 1,
                                Name = "Test Route 1",
                                Qualifications = new List<Qualification>
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
            },
            new()
            {
                UkPrn = 10000002,
                ProviderName = "Provider 2",
                LocationName = "Location 2",
                Postcode = "BB2 2BB",
                AddressLine1 = "Location 2 Address Line 1",
                AddressLine2 = "Location 2 Address Line",
                Town = "Location 2 Town",
                County = "Location 2 County",
                Email = "email.address@provider2.ac.uk",
                Telephone = "022 222 2222",
                Website = "https://www.provider2.ac.uk",
                Distance = 12.0,
                JourneyToLink = _searchOrigin?.CreateJourneyLink(new GeoLocation {Location = "BB2 2BB"}),
                DeliveryYears = new List<DeliveryYearSearchResult>
                {
                    new()
                    {
                        Year = 2022,
                        IsAvailableNow = false,
                        Routes = new List<Route>
                        {
                            new()
                            {
                                Id = 1,
                                Name = "Test Route 2",
                                Qualifications = new List<Qualification>
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
                        IsAvailableNow = false,
                        Routes = new List<Route>
                        {
                            new()
                            {
                                Id = 1,
                                Name = "Test Route 1",
                                Qualifications = new List<Qualification>
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
        };

    public IEnumerable<ProviderSearchResult> BuildListWithSingleItem() =>
        new List<ProviderSearchResult>
        {
            new()
            {
                UkPrn = 10000001,
                ProviderName = "Provider 1",
                LocationName = "Location 1",
                Postcode = "AA1 1AA",
                AddressLine1 = "Location 1 Address Line 1",
                AddressLine2 = "Location 1 Address Line",
                Town = "Location 1 Town",
                County = "Location 1 County",
                Email = "email.address@provider1.ac.uk",
                Telephone = "011 111 1111",
                Website = "https://www.provider1.ac.uk",
                Distance = 10.0,
                JourneyToLink = _searchOrigin?.CreateJourneyLink(new GeoLocation {Location = "AA1 1AA"}),
                DeliveryYears = new List<DeliveryYearSearchResult>
                {
                    new()
                    {
                        Year = 2021,
                        IsAvailableNow = true,
                        Routes = new List<Route>
                        {
                            new()
                            {
                                Id = 1,
                                Name = "Test Route 1",
                                Qualifications = new List<Qualification>
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
        };

    public IEnumerable<ProviderSearchResult> BuildProvidersPartOfListWithSingleItem() =>
        new List<ProviderSearchResult>
        {
            new()
            {
                UkPrn = 10000001,
                ProviderName = "Provider 1",
                LocationName = "Location 1",
                Postcode = "AA1 1AA",
                AddressLine1 = "Location 1 Address Line 1",
                AddressLine2 = "Location 1 Address Line",
                Town = "Location 1 Town",
                County = "Location 1 County",
                Email = "email.address@provider1.ac.uk",
                Telephone = "011 111 1111",
                Website = "https://www.provider1.ac.uk",
                Distance = 10.0,
                JourneyToLink = null
            }
        };

    public IEnumerable<DeliveryYearSearchResult> BuildDeliveryYearsPartOfListWithSingleItem() =>
        new List<DeliveryYearSearchResult>
        {
            new()
            {
                Year = 2021,
                IsAvailableNow = true
            }
        };

    public IEnumerable<RouteDto> BuildRoutesPartOfListWithSingleItem() =>
        new List<RouteDto>
        {
            new()
            {
                RouteId = 1,
                RouteName = "Test Route 1"
            }
        };

    public IEnumerable<QualificationDto> BuildQualificationsPartOfListWithSingleItem() =>
    new List<QualificationDto>
    {
            new()
            {
                QualificationId = 31,
                QualificationName = "Test Qualification 31"
            }
    };

    public ProviderSearchResult BuildSingleSearchResultWithSearchOrigin(GeoLocation origin) =>
        WithSearchOrigin(origin)
            .BuildListWithSingleItem()
            .First();

    public ProviderSearchResultBuilder WithSearchOrigin(GeoLocation origin)
    {
        _searchOrigin = origin;

        return this;
    }
}