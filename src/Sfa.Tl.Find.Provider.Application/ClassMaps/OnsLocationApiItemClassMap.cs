using CsvHelper.Configuration;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;
using System.Globalization;

// ReSharper disable StringLiteralTypo

namespace Sfa.Tl.Find.Provider.Application.ClassMaps;

public sealed class OnsLocationApiItemClassMap : ClassMap<OnsLocationApiItem>
{
    public OnsLocationApiItemClassMap()
    {
        Map(m => m.Id).Name("placeid");
        Map(m => m.LocationName).Name("place15nm");
        Map(m => m.CountyName).Name("cty15nm");
        Map(m => m.Country).Name("ctry15nm");
        Map(m => m.LocalAuthorityName).Name("ctyltnm");
        Map(m => m.LocalAuthorityDistrictDescription).Name("laddescnm");
        Map(m => m.LocalAuthorityDistrict).Convert(row =>
            Enum.TryParse<LocalAuthorityDistrict>(
                row.Row.GetField("laddescnm"), out var localAuthorityDistrict)
                ? localAuthorityDistrict : default);
        Map(m => m.LocationAuthorityDistrict).Name("lad15nm");
        Map(m => m.PlaceNameDescription).Name("descnm");
        Map(m => m.PlaceName).Convert(row =>
            Enum.TryParse<PlaceNameDescription>(
                row.Row.GetField("descnm"), out var placeName)
                    ? placeName : default);
        Map(m => m.Latitude).Name("lat")
            .TypeConverterOption
            .NumberStyles(NumberStyles.Number | NumberStyles.AllowExponent);
        Map(m => m.Longitude).Name("long")
            .TypeConverterOption
            .NumberStyles(NumberStyles.Number | NumberStyles.AllowExponent);
        Map(m => m.PopulationCount).Name("popcnt");
    }
}
