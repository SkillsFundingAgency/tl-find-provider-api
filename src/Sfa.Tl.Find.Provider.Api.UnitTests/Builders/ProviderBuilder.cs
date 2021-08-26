using System.Collections.Generic;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    public class ProviderBuilder
    {
        public IEnumerable<Models.Provider> BuildList() =>
            new List<Models.Provider>
            {
                new()
                {
                    UkPrn = 10000001,
                    Name = "Provider 1",
                    Postcode = "AA1 1AA",
                    AddressLine1 = "Provider 1 Address Line 1",
                    AddressLine2 = "Provider 1 Address Line",
                    Town = "Provider 1 Town",
                    County = "Provider 1 County",
                    Email = "email.address@provider1.ac.uk",
                    Telephone = "011 111 1111",
                    Website= "https://www.provider1.ac.uk",
                    Locations = new List<Location>
                    {
                        new()
                        {
                            Postcode = "AA1 1AA",
                            AddressLine1 = "Location 1 Address Line 1",
                            AddressLine2 = "Location 1 Address Line",
                            Town = "Location 1 Town",
                            County = "Location 1 County",
                            Email = "email.address@provider1.ac.uk",
                            Telephone = "011 111 1111",
                            Website= "https://www.provider1.ac.uk",
                            Latitude = 50.0,
                            Longitude = 1.0,
                            DeliveryYears = new List<DeliveryYear>
                            {
                                new()
                                {
                                    Year = 2021,
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
                        }
                    }
                },
                new()
                {
                    UkPrn = 10000002,
                    Name = "Provider 2",
                    AddressLine1 = "Provider 2 Address Line 1",
                    AddressLine2 = "Provider 2 Address Line",
                    Town = "Provider 2 Town",
                    County = "Provider 2 County",
                    Postcode = "BB2 2BB",
                    Email = "email.address@provider2.ac.uk",
                    Telephone = "022 222 2222",
                    Website= "https://www.provider2.ac.uk",

                    Locations = new List<Location>()
                }
            };
    }
}
