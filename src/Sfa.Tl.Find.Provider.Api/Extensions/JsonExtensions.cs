using System.Text.Json;

namespace Sfa.Tl.Find.Provider.Api.Extensions
{
    public static class JsonExtensions
    {
        public static bool SafeGetBoolean(this JsonElement element, 
            string propertyName)
        {
            return element.TryGetProperty(propertyName, out var property)
                   && property.ValueKind is JsonValueKind.True or JsonValueKind.False
                   && property.GetBoolean();
        }

        public static int SafeGetInt32(this JsonElement element, 
            string propertyName, 
            int defaultValue = default)
        {
            return element.TryGetProperty(propertyName, out var property)
                   && property.ValueKind == JsonValueKind.Number
                   && property.TryGetInt32(out var val)
                ? val
                : defaultValue;
        }

        public static long SafeGetInt64(this JsonElement element, 
            string propertyName, 
            long defaultValue = default)
        {
            return element.TryGetProperty(propertyName, out var property)
                   && property.ValueKind == JsonValueKind.Number
                   && property.TryGetInt64(out var val)
                ? val
                : defaultValue;
        }

        public static double SafeGetDouble(this JsonElement element, 
            string propertyName, 
            double defaultValue = default)
        {
            return element.TryGetProperty(propertyName, out var property)
                   && property.ValueKind == JsonValueKind.Number
                   && property.TryGetDouble(out var val)
                ? val
                : defaultValue;
        }

        public static string SafeGetString(this JsonElement element, 
            string propertyName, 
            int maxLength = -1, 
            string defaultValue = default)
        {
            var result = element.TryGetProperty(propertyName, out var property)
                         && property.ValueKind == JsonValueKind.String
                ? property.GetString()
                : defaultValue;

            return result is not null && maxLength > 0 && result.Length > maxLength
                ? result[..maxLength].Trim()
                : result;
        }
    }
}
