using System.Collections.Generic;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Models;

public class DeliveryYearBuilder
{
    public IEnumerable<DeliveryYear> BuildList() =>
        new List<DeliveryYear>
        {
            new()
            {
                Year = 2021,
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
            },
            new()
            {
                Year = 2022,
                Qualifications = new List<Qualification>
                {
                    new()
                    {
                        Id = 41,
                        Name = "Test Qualification 41"
                    },
                    new()
                    {
                        Id = 42,
                        Name = "Test Qualification 42"
                    }
                }
            }
        };
}