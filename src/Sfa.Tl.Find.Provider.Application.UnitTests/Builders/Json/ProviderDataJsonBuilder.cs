using Sfa.Tl.Find.Provider.Application.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Json;

public static class ProviderDataJsonBuilder
{
    private const string AssetFolderPath = "Assets.ProviderData";

    public static Stream BuildProviderDataStream() =>
        typeof(ProviderDataJsonBuilder)
            .ReadManifestResourceStream(
                $"{AssetFolderPath}.ProviderData.json");
}