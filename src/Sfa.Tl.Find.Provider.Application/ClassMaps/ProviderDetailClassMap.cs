using CsvHelper.Configuration;
using Sfa.Tl.Find.Provider.Application.Models;
using System.Diagnostics.Metrics;
using System.Net;

namespace Sfa.Tl.Find.Provider.Application.ClassMaps;

public sealed class ProviderDetailClassMap : ClassMap<ProviderDetail>
{
    public ProviderDetailClassMap()
    {
        Map(p => p.UkPrn).Name("UKPRN");
        Map(p => p.Name).Name("Provider Name");
        Map(p => p.Postcode).Name("Postcode");
        //TODO: Need to flatten locations, routes, qualifications
        //Map(p => p.Locations).Name("Location Name");
        //Map(p => p.Locations);
        Map(p => p.AddressLine1).Name("Address Line 1");
        Map(p => p.AddressLine2).Name(".Address Line 2");
        Map(p => p.Town).Name("Town");
        Map(p => p.County).Name("County");
        Map(p => p.Email).Name("Email");
        Map(p => p.Telephone).Name(".Telephone");
        Map(p => p.Website).Name(".Website");
        //Map(p => p.DeliveryYear).Name("Year of Delivery");
        //Map(p => p.RouteName).Name("Route Name");"
        //Map(p => p.QualificationName).Name("Qualification Name");
        //Map(p => p.PayloadCapacity).Name("payload_capacity").Convert(rocket =>
        //{
        //    return rocket.PayloadCapacity.HasValue ? $"{rocket.PayloadCapacity} kg" : String.Empty;
        //});
        //Map(p => p.FirstLaunchDate).Name("first_launch_date").TypeConverterOption.Format("s");
    }
}

public sealed class LocationClassMap : ClassMap<Location>
{
    public LocationClassMap()
    {
        Map(l => l.Name).Name("Location Name");
        Map(l => l.Postcode).Name("Postcode");
    }
}
