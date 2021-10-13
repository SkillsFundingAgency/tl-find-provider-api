using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions
{
    public static class ProviderSearchResponseExtensions
    {
        public static async Task<ProviderSearchResponse> DeserializeFromHttpContent(this HttpContent content)
        {
            return JsonSerializer.Deserialize<ProviderSearchResponse>(
                await content.ReadAsStringAsync(),
                new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
        }
    }
}
