using Sfa.Tl.Find.Provider.Application.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Csv;

public static class ProviderContactsCsvBuilder
{
    private const string AssetFolderPath = "Assets";
    
    public static Stream BuildProviderContactsCsvAsStream() =>
        typeof(ProviderContactsCsvBuilder)
            .ReadManifestResourceStream(
                $"{AssetFolderPath}.ProviderContacts.csv");
}