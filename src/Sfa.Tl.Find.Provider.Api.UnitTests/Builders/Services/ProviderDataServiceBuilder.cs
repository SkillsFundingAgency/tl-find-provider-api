using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Models;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Application.Services;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Services;

public class ProviderDataServiceBuilder
{
    public ProviderDataService Build(
        IDateTimeService dateTimeService = null,
        IPostcodeLookupService postcodeLookupService = null,
        IProviderRepository providerRepository = null,
        IQualificationRepository qualificationRepository = null,
        IRouteRepository routeRepository = null,
        ITownDataService townDataService = null,
        IMemoryCache cache = null,
        SearchSettings searchSettings = null,
        ILogger<ProviderDataService> logger = null)
    {
        dateTimeService ??= Substitute.For<IDateTimeService>();
        postcodeLookupService ??= Substitute.For<IPostcodeLookupService>();
        providerRepository ??= Substitute.For<IProviderRepository>();
        qualificationRepository ??= Substitute.For<IQualificationRepository>();
        routeRepository ??= Substitute.For<IRouteRepository>();
        townDataService ??= Substitute.For<ITownDataService>();
        cache ??= Substitute.For<IMemoryCache>();
        logger ??= Substitute.For<ILogger<ProviderDataService>>();

        searchSettings ??= new SettingsBuilder().BuildSearchSettings();
        var searchOptions = new Func<IOptions<SearchSettings>>(() =>
        {
            var options = Substitute.For<IOptions<SearchSettings>>();
            options.Value.Returns(searchSettings);
            return options;
        }).Invoke();

        return new ProviderDataService(
            dateTimeService,
            postcodeLookupService,
            providerRepository,
            qualificationRepository,
            routeRepository,
            townDataService,
            cache,
            searchOptions,
            logger);
    }
}