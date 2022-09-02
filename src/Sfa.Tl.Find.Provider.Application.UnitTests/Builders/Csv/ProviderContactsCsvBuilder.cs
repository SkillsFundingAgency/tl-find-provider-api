using Sfa.Tl.Find.Provider.Application.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Csv;

public static class ProviderContactsCsvBuilder
{
    private const string AssetFolderPath = "Assets";

    public static string BuildProviderContactsCsv() =>
        typeof(ProviderContactsCsvBuilder)
            .ReadManifestResourceStreamAsString(
                $"{AssetFolderPath}.ProviderContacts.csv");

    public static Stream BuildProviderContactsCsvAsStream() =>
        typeof(ProviderContactsCsvBuilder)
            .ReadManifestResourceStream(
                $"{AssetFolderPath}.ProviderContacts.csv");

    public static byte[] BuildProviderContactsCsvAsBytes() =>
        typeof(ProviderContactsCsvBuilder)
            .ReadManifestResourceStreamAsBytes(
                $"{AssetFolderPath}.ProviderContacts.csv");
}