namespace Sfa.Tl.Find.Provider.Tests.Common.Extensions;

public static class TokenDictionaryValidationExtensions
{
    public static bool ValidateTokens(this IDictionary<string, string> tokens, IDictionary<string, string> expectedTokens)
    {
        foreach (var (key, value) in expectedTokens)
        {
            tokens.Should().ContainKey(key);
            tokens[key].Should().Be(value, 
                $"this is the expected value for key '{key}'");
        }

        return true;
    }
}