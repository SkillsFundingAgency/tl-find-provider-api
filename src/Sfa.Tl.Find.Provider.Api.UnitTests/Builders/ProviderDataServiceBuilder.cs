using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Services;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    public class ProviderDataServiceBuilder
    {
        public ProviderDataService Build(
            IProviderRepository providerRepository = null,
            IQualificationRepository qualificationRepository = null,
            ILogger<ProviderDataService> logger = null)
        {
            logger ??= Substitute.For<ILogger<ProviderDataService>>();
            providerRepository ??= Substitute.For<IProviderRepository>();
            qualificationRepository ??= Substitute.For<IQualificationRepository>();

            return new ProviderDataService(providerRepository, qualificationRepository, logger);
        }
    }
}
