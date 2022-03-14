using System;
using System.IO;
using System.Reflection;

namespace Sfa.Tl.Find.Provider.Api.Extensions;

public static class ResourceExtensions
{
    public static string BuildJsonFromResourceStream(this Type type, string assetName) =>
        type
            .ReadManifestResourceStreamAsString(
                $"Assets.{assetName}.json");

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