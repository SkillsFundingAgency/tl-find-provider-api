using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Web.Pages.Provider;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages.Provider;
public  class SearchFiltersTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(SearchFiltersModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task SearchFiltersModel_OnGet_Sets_ExpectedProperties()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var searchFiltersModel = new SearchFiltersModelBuilder()
            .Build(providerSettings: settings);

        await searchFiltersModel.OnGet();
    }
}
