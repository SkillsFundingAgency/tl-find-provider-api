﻿using Microsoft.Extensions.Logging;
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
        IDateTimeService dateTimeService = null,
        IPostcodeLookupService postcodeLookupService = null,
        IProviderRepository providerRepository = null,
        IQualificationRepository qualificationRepository = null,
        IRouteRepository routeRepository = null,
        IIndustryRepository industryRepository = null,
        ITownDataService townDataService = null,
        ICacheService cacheService = null,
        SearchSettings searchSettings = null,
        ILogger<ProviderDataService> logger = null)
    {
        dateTimeService ??= Substitute.For<IDateTimeService>();
        postcodeLookupService ??= Substitute.For<IPostcodeLookupService>();
        providerRepository ??= Substitute.For<IProviderRepository>();
        qualificationRepository ??= Substitute.For<IQualificationRepository>();
        routeRepository ??= Substitute.For<IRouteRepository>();
        industryRepository ??= Substitute.For<IIndustryRepository>();
        townDataService ??= Substitute.For<ITownDataService>();
        cacheService ??= Substitute.For<ICacheService>();
        logger ??= Substitute.For<ILogger<ProviderDataService>>();

        searchSettings ??= new SettingsBuilder().BuildSearchSettings();
        var searchOptions = searchSettings
            .ToOptions();

        return new ProviderDataService(
            dateTimeService,
            postcodeLookupService,
            providerRepository,
            qualificationRepository,
            routeRepository,
            industryRepository,
            townDataService,
            cacheService,
            searchOptions,
            logger);
    }
}