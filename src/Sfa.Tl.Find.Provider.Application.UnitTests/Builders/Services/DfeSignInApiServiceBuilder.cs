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
        DfeSignInSettings signInSettings = null)
    {
        httpClient ??= Substitute.For<HttpClient>();
        var tokenService = new DfeSignInTokenServiceBuilder()
            .Build();

        signInSettings ??= new SettingsBuilder().BuildDfeSignInSettings();
        var signInOptions = signInSettings.ToOptions();

        return new DfeSignInApiService(
            httpClient,
            tokenService,
            signInOptions);
    }
}
