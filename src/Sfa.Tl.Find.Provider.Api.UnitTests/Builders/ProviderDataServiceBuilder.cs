using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Services;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    public class ProviderDataServiceBuilder
    {
        public ProviderDataService Build(
            ILogger<ProviderDataService> logger = null)
        {
            logger ??= Substitute.For<ILogger<ProviderDataService>>();

            return new ProviderDataService(logger);
        }
    }
}
