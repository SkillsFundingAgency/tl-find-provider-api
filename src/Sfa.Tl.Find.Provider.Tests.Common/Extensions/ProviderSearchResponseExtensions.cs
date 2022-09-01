using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sfa.Tl.Find.Provider.Tests.Common.Extensions;

public static class ProviderSearchResponseExtensions
{
    public static async Task<T> DeserializeFromHttpContent<T>(this HttpContent content)
        where T: class
    {
        return JsonSerializer.Deserialize<T>(
            await content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
    }
}