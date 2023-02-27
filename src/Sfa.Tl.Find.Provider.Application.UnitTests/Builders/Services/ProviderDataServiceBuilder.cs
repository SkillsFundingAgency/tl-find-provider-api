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
        IPostcodeLookupService postcodeLookupService = null,
        IEmailService emailService = null,
        IProviderRepository providerRepository = null,
        IQualificationRepository qualificationRepository = null,
        IRouteRepository routeRepository = null,
        IIndustryRepository industryRepository = null,
        ITownDataService townDataService = null,
        ICacheService cacheService = null,
        ProviderSettings providerSettings = null,
        ILogger<ProviderDataService> logger = null)
    {
        postcodeLookupService ??= Substitute.For<IPostcodeLookupService>();
        emailService ??= Substitute.For<IEmailService>();
        providerRepository ??= Substitute.For<IProviderRepository>();
        qualificationRepository ??= Substitute.For<IQualificationRepository>();
        routeRepository ??= Substitute.For<IRouteRepository>();
        industryRepository ??= Substitute.For<IIndustryRepository>();
        townDataService ??= Substitute.For<ITownDataService>();
        cacheService ??= Substitute.For<ICacheService>();
        logger ??= Substitute.For<ILogger<ProviderDataService>>();

        providerSettings ??= new SettingsBuilder().BuildProviderSettings();

        return new ProviderDataService(
            postcodeLookupService,
            emailService,
            providerRepository,
            qualificationRepository,
            routeRepository,
            industryRepository,
            townDataService,
            cacheService,
            providerSettings.ToOptions(),
            logger);
    }
}