using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Services;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    public class ProviderDataServiceBuilder
    {
        public ProviderDataService Build(
            IDateTimeService dateTimeService = null,
            IPostcodeLookupService postcodeLookupService = null,
            IProviderRepository providerRepository = null,
            IQualificationRepository qualificationRepository = null,
            IMemoryCache cache = null,
            ILogger<ProviderDataService> logger = null)
        {
            dateTimeService ??= Substitute.For<IDateTimeService>();
            postcodeLookupService ??= Substitute.For<IPostcodeLookupService>();
            providerRepository ??= Substitute.For<IProviderRepository>();
            qualificationRepository ??= Substitute.For<IQualificationRepository>();
            cache ??= Substitute.For<IMemoryCache>();
            logger ??= Substitute.For<ILogger<ProviderDataService>>();

            return new ProviderDataService(
                dateTimeService,
                postcodeLookupService,
                providerRepository, 
                qualificationRepository, 
                cache,
                logger);
        }
    }
}
