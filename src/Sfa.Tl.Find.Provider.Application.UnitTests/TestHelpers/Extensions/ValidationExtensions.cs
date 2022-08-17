
namespace Sfa.Tl.Find.Provider.Application.UnitTests.TestHelpers.Extensions;
public static class ValidationExtensions
{
    public static bool HasExpectedValue(this IDictionary<string, dynamic> dictionary, string key, string expectedValue)
    {
        return dictionary.ContainsKey(key) && (string)dictionary[key] == expectedValue;
    }
}
