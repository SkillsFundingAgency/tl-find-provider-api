using CsvHelper.Configuration;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.ClassMaps;

public sealed class ProviderDetailFlatClassMap : ClassMap<ProviderDetailFlat>
{
    public ProviderDetailFlatClassMap()
    {
        Map(p => p.UkPrn).Name("UKPRN");
        Map(p => p.ProviderName).Name("Provider Name");
        Map(p => p.Postcode).Name("Postcode");
        Map(p => p.AddressLine1).Name("Address Line 1");
        Map(p => p.AddressLine2).Name(".Address Line 2");
        Map(p => p.Town).Name("Town");
        Map(p => p.County).Name("County");
        Map(p => p.Email).Name("Email");
        Map(p => p.Telephone).Name(".Telephone");
        Map(p => p.Website).Name(".Website");
        Map(p => p.Year).Name("Year of Delivery");
        Map(p => p.RouteName).Name("Route Name");
        Map(p => p.QualificationName).Name("Qualification Name");
    }
}
