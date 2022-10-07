using Sfa.Tl.Find.Provider.Application.Interfaces;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using JWT.Algorithms;
using JWT.Builder;

namespace Sfa.Tl.Find.Provider.Application.Services;
public class DfeSignInTokenService : IDfeSignInTokenService
{
    private readonly DfeSignInSettings _signInSettings;

    public DfeSignInTokenService(
        IOptions<DfeSignInSettings> signInOptions)
    {
        if (signInOptions is null) throw new ArgumentNullException(nameof(signInOptions));

        _signInSettings = signInOptions.Value;
    }

    public string GetApiToken() =>
        new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .Issuer(_signInSettings.Issuer)
            .Audience(_signInSettings.Audience)
            .WithSecret(_signInSettings.ApiSecret)
            .Encode();
}
