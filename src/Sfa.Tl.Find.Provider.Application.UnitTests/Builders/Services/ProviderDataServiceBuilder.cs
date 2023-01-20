using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;

public class ProviderDataServiceBuilder
{
    public IProviderDataService Build(
        IDateTimeProvider dateTimeProvider = null,
        IPostcodeLookupService postcodeLookupService = null,
        IEmailService emailService = null,
        IProviderRepository providerRepository = null,
        IQualificationRepository qualificationRepository = null,
        IRouteRepository routeRepository = null,
        IIndustryRepository industryRepository = null,
        INotificationRepository notificationRepository = null,
        ISearchFilterRepository searchFilterRepository = null,
        ITownDataService townDataService = null,
        ICacheService cacheService = null,
        SearchSettings searchSettings = null,
        ILogger<ProviderDataService> logger = null)
    {
        dateTimeProvider ??= Substitute.For<IDateTimeProvider>();
        postcodeLookupService ??= Substitute.For<IPostcodeLookupService>();
        emailService ??= Substitute.For<IEmailService>();
        providerRepository ??= Substitute.For<IProviderRepository>();
        qualificationRepository ??= Substitute.For<IQualificationRepository>();
        routeRepository ??= Substitute.For<IRouteRepository>();
        industryRepository ??= Substitute.For<IIndustryRepository>();
        notificationRepository ??= Substitute.For<INotificationRepository>();
        searchFilterRepository ??= Substitute.For<ISearchFilterRepository>();
        townDataService ??= Substitute.For<ITownDataService>();
        cacheService ??= Substitute.For<ICacheService>();
        logger ??= Substitute.For<ILogger<ProviderDataService>>();

        searchSettings ??= new SettingsBuilder().BuildSearchSettings();
        var searchOptions = searchSettings
            .ToOptions();

        return new ProviderDataService(
            dateTimeProvider,
            postcodeLookupService,
            emailService,
            providerRepository,
            qualificationRepository,
            routeRepository,
            industryRepository,
            notificationRepository,
            searchFilterRepository,
            townDataService,
            cacheService,
            searchOptions,
            logger);
    }
}