using System.Reflection;

namespace Sfa.Tl.Find.Provider.Application.Extensions;

public static class ResourceExtensions
{
    public static string BuildJsonFromResourceStream(this Type type, string assetFolderPath, string assetName) =>
    type
    .ReadManifestResourceStreamAsString(
        $"{assetFolderPath}.{assetName}.json");

    public static string ReadManifestResourceStreamAsString(this Type type, string resourcePath)
    {
        return type.Assembly.ReadManifestResourceStreamAsString($"{type.Namespace}.{resourcePath}");
    }

    public static string ReadManifestResourceStreamAsString(this string relativeResourcePath)
    {
        var assembly = Assembly.GetCallingAssembly();
        return assembly.ReadManifestResourceStreamAsString($"{assembly.GetName().Name}.{relativeResourcePath}");
    }

    public static string ReadManifestResourceStreamAsString(this Assembly assembly, string resourcePath)
    {
        using var stream = assembly.GetManifestResourceStream(resourcePath);

        if (stream == null)
        {
            throw new Exception($"Stream for '{resourcePath}' not found.");
        }

        using var stringReader = new StreamReader(stream);
        return stringReader.ReadToEnd();
    }
}