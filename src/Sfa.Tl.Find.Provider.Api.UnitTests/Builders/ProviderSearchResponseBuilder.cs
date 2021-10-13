using System.Collections.Generic;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    public class ProviderSearchResponseBuilder
    {
        private PostcodeLocation _searchOrigin;
        
        private const string DefaultPostcode = "CV1 2WT";
        
        public ProviderSearchResponse BuildWithMultipleSearchResults() =>
            new()
            {
                Postcode = _searchOrigin != null ? _searchOrigin.Postcode : DefaultPostcode,
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
                        JourneyToLink = _searchOrigin.CreateJourneyLink(new PostcodeLocation { Postcode = "AA1 1AA" }),
                        DeliveryYears = new List<DeliveryYear>
                        {
                            new()
                            {
                                Year = 2021,
                                IsAvailableNow = true,
                                Qualifications = new List<Qualification>()
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
                        JourneyToLink = _searchOrigin.CreateJourneyLink(new PostcodeLocation { Postcode = "BB2 2BB" }),
                        DeliveryYears = new List<DeliveryYear>
                        {
                            new()
                            {
                                Year = 2022,
                                IsAvailableNow = false,
                                Qualifications = new List<Qualification>()
                                {
                                    new()
                                    {
                                        Id = 51,
                                        Name = "Test Qualification 51"
                                    }
                                }
                            },
                            new()
                            {
                                Year = 2023,
                                IsAvailableNow = false,
                                Qualifications = new List<Qualification>()
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
            };

        public ProviderSearchResponse BuildWithSingleSearchResult() =>
            new()
            {
                Postcode = _searchOrigin != null ? _searchOrigin.Postcode : DefaultPostcode,
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
                        JourneyToLink = _searchOrigin.CreateJourneyLink(new PostcodeLocation { Postcode = "AA1 1AA" }),
                        DeliveryYears = new List<DeliveryYear>
                        {
                            new()
                            {
                                Year = 2021,
                                IsAvailableNow = true,
                                Qualifications = new List<Qualification>()
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
            };

        public ProviderSearchResponse BuildErrorResponse(string errorMessage) =>
            new()
            {
                Error = errorMessage
            };

        public ProviderSearchResponseBuilder WithSearchOrigin(PostcodeLocation origin)
        {
            _searchOrigin = origin;

            return this;
        }
    }
}
