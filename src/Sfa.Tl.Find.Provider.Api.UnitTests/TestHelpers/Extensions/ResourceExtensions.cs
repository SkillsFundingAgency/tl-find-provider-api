using System;
using System.IO;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions
{
    public static class ResourceExtensions
    {
        public static string ReadManifestResourceStreamAsString(this Type type, string resourcePath)
        {
            using var stream = type.Assembly.GetManifestResourceStream($"{type.Namespace}.{resourcePath}");

            if (stream == null)
            {
                throw new Exception($"Stream for '{resourcePath}' not found.");
            }

            using var stringReader = new StreamReader(stream);
            return stringReader.ReadToEnd();
        }
    }
}
