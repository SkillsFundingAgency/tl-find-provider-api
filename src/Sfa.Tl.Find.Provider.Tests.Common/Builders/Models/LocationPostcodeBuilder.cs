using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class LocationPostcodeBuilder
{
    public LocationPostcode Build() =>
        BuildList()
            .First();

    public IEnumerable<LocationPostcode> BuildList() =>
        new List<LocationPostcode>
        {
            new()
            {
                Postcode = "AA1 1AA",
                Name = "Test Location A",
                Latitude = 50.0,
                Longitude = 1.0
            },
            new()
            {
                Postcode = "BB1 1BB",
                Name = "Test Location B",
                Latitude = 51.0,
                Longitude = -1.0
            }
        };
}