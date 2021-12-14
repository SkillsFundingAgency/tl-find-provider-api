using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;

public static class ProviderSearchResponseExtensions
{
    public static async Task<ProviderSearchResponse> DeserializeFromHttpContent(this HttpContent content)
    {
        return JsonSerializer.Deserialize<ProviderSearchResponse>(
            await content.ReadAsStringAsync(),
            new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
    }
}