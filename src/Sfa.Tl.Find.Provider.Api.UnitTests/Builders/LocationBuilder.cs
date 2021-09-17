using System.Collections.Generic;
using System.Linq;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    public class LocationBuilder
    {
        public Location Build() =>
            BuildList().First();

        public IEnumerable<Location> BuildList() =>
            new List<Location>
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
                },
                new()
                {
                    Postcode = "BB1 1BB",
                    AddressLine1 = "Location 2 Address Line 1",
                    AddressLine2 = "Location 2 Address Line",
                    Town = "Location 2 Town",
                    County = "Location 2 County",
                    Email = "email.address@provider1.ac.uk",
                    Telephone = "011 111 1111",
                    Website= "https://www.provider2.ac.uk",
                    Latitude = 51.0,
                    Longitude = -1.0,
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
            };
    }
}
