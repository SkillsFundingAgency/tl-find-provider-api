using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Services;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    public class ProviderDataServiceBuilder
    {
        public ProviderDataService Build(
            IPostcodeLookupService postcodeLookupService = null,
            IProviderRepository providerRepository = null,
            IQualificationRepository qualificationRepository = null,
            ILogger<ProviderDataService> logger = null)
        {
            postcodeLookupService ??= Substitute.For<IPostcodeLookupService>();
            providerRepository ??= Substitute.For<IProviderRepository>();
            qualificationRepository ??= Substitute.For<IQualificationRepository>();
            logger ??= Substitute.For<ILogger<ProviderDataService>>();

            return new ProviderDataService(
                postcodeLookupService, 
                providerRepository, 
                qualificationRepository, 
                logger);
        }
    }
}
