using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
public class DfeSignInApiServiceBuilder
{
    public IDfeSignInApiService Build(
        HttpClient httpClient = null,
        DfeSignInSettings signInSettings = null,
        ILogger<DfeSignInApiService> logger = null)
    {
        httpClient ??= Substitute.For<HttpClient>();
        var tokenService = new DfeSignInTokenServiceBuilder()
            .Build();

        logger ??= Substitute.For<ILogger<DfeSignInApiService>>();
        signInSettings ??= new SettingsBuilder().BuildDfeSignInSettings();
        var signInOptions = signInSettings.ToOptions();

        return new DfeSignInApiService(
            httpClient,
            tokenService,
            signInOptions,
            logger);
    }
}
