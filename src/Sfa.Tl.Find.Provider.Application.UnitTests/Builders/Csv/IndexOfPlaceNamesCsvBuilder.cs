using Sfa.Tl.Find.Provider.Application.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Csv;

public static class IndexOfPlaceNamesCsvBuilder
{
    private const string AssetFolderPath = "Assets";
    
    public static Stream BuildIndexOfPlaceNamesCsvAsStream() =>
        typeof(IndexOfPlaceNamesCsvBuilder)
            .ReadManifestResourceStream(
                $"{AssetFolderPath}.IndexOfPlaceNames.csv");
}