using System.Collections.Generic;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Models;

public class ProviderSearchResponseBuilder
{
    private GeoLocation _searchOrigin;
    private int? _totalSearchResults;

    private const string DefaultPostcode = "CV1 2WT";
    private const int DefaultTotalSearchResults = 10;

    public ProviderSearchResponse BuildWithMultipleSearchResults() =>
        new()
        {
            SearchTerm = _searchOrigin != null ? _searchOrigin.Location : DefaultPostcode,
            TotalResults = _totalSearchResults ?? DefaultTotalSearchResults,
            SearchResults = new List<ProviderSearchResult>
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
                    JourneyToLink = _searchOrigin.CreateJourneyLink(new GeoLocation { Location = "AA1 1AA" }),
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
                    JourneyToLink = _searchOrigin.CreateJourneyLink(new GeoLocation { Location = "BB2 2BB" }),
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
                                    Name = "Test Route 3",
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
            }
        };

    public ProviderSearchResponse BuildWithSingleSearchResult() =>
        new()
        {
            SearchTerm = _searchOrigin != null ? _searchOrigin.Location : DefaultPostcode,
            SearchResults = new List<ProviderSearchResult>
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
                    JourneyToLink = _searchOrigin.CreateJourneyLink(new GeoLocation { Location = "AA1 1AA" }),
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
            },
            TotalResults = 1
        };

    public ProviderSearchResponse BuildErrorResponse(string errorMessage) =>
        new()
        {
            Error = errorMessage
        };

    public ProviderSearchResponseBuilder WithSearchOrigin(GeoLocation origin)
    {
        _searchOrigin = origin;

        return this;
    }

    public ProviderSearchResponseBuilder WithTotalSearchResults(int totalSearchResults)
    {
        _totalSearchResults = totalSearchResults;

        return this;
    }

}