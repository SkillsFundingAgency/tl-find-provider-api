using Sfa.Tl.Find.Provider.Application.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Json;

public static class DfeSignInApiJsonBuilder
{
    private const string AssetFolderPath = "Assets.DfeSignInApi";

    public static string BuildOrganisationsResponse() =>
        typeof(GoogleApiJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath,
                "dfe_organisations");

    public static string BuildUserResponse() =>
        typeof(GoogleApiJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath,
                "dfe_user");
}